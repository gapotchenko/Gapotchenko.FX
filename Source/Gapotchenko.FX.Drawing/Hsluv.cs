﻿using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Drawing
{
    static class Hsluv
    {
        static readonly double[][] M = new double[][]
        {
            new double[] {  3.240969941904521, -1.537383177570093, -0.498610760293    },
            new double[] { -0.96924363628087,   1.87596750150772,   0.041555057407175 },
            new double[] {  0.055630079696993, -0.20397695888897,   1.056971514242878 },
        };

        static readonly double[][] MInv = new double[][]
        {
            new double[] { 0.41239079926595,  0.35758433938387, 0.18048078840183  },
            new double[] { 0.21263900587151,  0.71516867876775, 0.072192315360733 },
            new double[] { 0.019330818715591, 0.11919477979462, 0.95053215224966  },
        };

        const double RefX = 0.95045592705167;
        const double RefY = 1.0;
        const double RefZ = 1.089057750759878;

        const double RefU = 0.19783000664283;
        const double RefV = 0.46831999493879;

        const double Kappa = 903.2962962;
        const double Epsilon = 0.0088564516;

        static IList<double[]> GetBounds(double L)
        {
            var result = new List<double[]>();

            double sub1 = Math.Pow(L + 16, 3) / 1560896;
            double sub2 = sub1 > Epsilon ? sub1 : L / Kappa;

            for (int c = 0; c < 3; ++c)
            {
                var m1 = M[c][0];
                var m2 = M[c][1];
                var m3 = M[c][2];

                for (int t = 0; t < 2; ++t)
                {
                    var top1 = (284517 * m1 - 94839 * m3) * sub2;
                    var top2 = (838422 * m3 + 769860 * m2 + 731718 * m1) * L * sub2 - 769860 * t * L;
                    var bottom = (632260 * m3 - 126452 * m2) * sub2 + 126452 * t;

                    result.Add(new double[] { top1 / bottom, top2 / bottom });
                }
            }

            return result;
        }

        static double IntersectLineLine(IList<double> lineA,
            IList<double> lineB)
        {
            return (lineA[1] - lineB[1]) / (lineB[0] - lineA[0]);
        }

        static double DistanceFromPole(IList<double> point)
        {
            return Math.Sqrt(Math.Pow(point[0], 2) + Math.Pow(point[1], 2));
        }

        static bool LengthOfRayUntilIntersect(double theta,
            IList<double> line,
            out double length)
        {
            length = line[1] / (Math.Sin(theta) - line[0] * Math.Cos(theta));

            return length >= 0;
        }

        static double MaxSafeChromaForL(double L)
        {
            var bounds = GetBounds(L);
            double min = Double.MaxValue;

            for (int i = 0; i < 2; ++i)
            {
                var m1 = bounds[i][0];
                var b1 = bounds[i][1];
                var line = new double[] { m1, b1 };

                double x = IntersectLineLine(line, new double[] { -1 / m1, 0 });
                double length = DistanceFromPole(new double[] { x, b1 + x * m1 });

                min = Math.Min(min, length);
            }

            return min;
        }

        static double MaxChromaForLH(double L, double H)
        {
            double hrad = H / 360 * Math.PI * 2;

            var bounds = GetBounds(L);
            double min = Double.MaxValue;

            foreach (var bound in bounds)
            {
                double length;

                if (LengthOfRayUntilIntersect(hrad, bound, out length))
                {
                    min = Math.Min(min, length);
                }
            }

            return min;
        }

        static double DotProduct(IList<double> a,
            IList<double> b)
        {
            double sum = 0;

            for (int i = 0; i < a.Count; ++i)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }

        static double Round(double value, int places)
        {
            double n = Math.Pow(10, places);

            return Math.Round(value * n) / n;
        }

        static double FromLinear(double c)
        {
            if (c <= 0.0031308)
            {
                return 12.92 * c;
            }
            else
            {
                return 1.055 * Math.Pow(c, 1 / 2.4) - 0.055;
            }
        }

        static double ToLinear(double c)
        {
            if (c > 0.04045)
            {
                return Math.Pow((c + 0.055) / (1 + 0.055), 2.4);
            }
            else
            {
                return c / 12.92;
            }
        }

        static IList<int> RgbPrepare(IList<double> tuple)
        {

            for (int i = 0; i < tuple.Count; ++i)
            {
                tuple[i] = Round(tuple[i], 3);
            }

            for (int i = 0; i < tuple.Count; ++i)
            {
                double ch = tuple[i];

                if (ch < -0.0001 || ch > 1.0001)
                    throw new ArgumentException("Illegal RGB value: " + ch);
            }

            var results = new int[tuple.Count];

            for (int i = 0; i < tuple.Count; ++i)
            {
                results[i] = (int)Math.Round(tuple[i] * 255);
            }

            return results;
        }

        public static IList<double> XyzToRgb(IList<double> tuple)
        {
            return new double[]
            {
                FromLinear(DotProduct(M[0], tuple)),
                FromLinear(DotProduct(M[1], tuple)),
                FromLinear(DotProduct(M[2], tuple)),
            };
        }

        public static IList<double> RgbToXyz(IList<double> tuple)
        {
            var rgbl = new double[]
            {
                ToLinear(tuple[0]),
                ToLinear(tuple[1]),
                ToLinear(tuple[2]),
            };

            return new double[]
            {
                DotProduct(MInv[0], rgbl),
                DotProduct(MInv[1], rgbl),
                DotProduct(MInv[2], rgbl),
            };
        }

        static double YToL(double Y)
        {
            if (Y <= Epsilon)
            {
                return (Y / RefY) * Kappa;
            }
            else
            {
                return 116 * Math.Pow(Y / RefY, 1.0 / 3.0) - 16;
            }
        }

        /// <summary>
        /// Converts lightness to luminance.
        /// </summary>
        /// <param name="L">The lightness.</param>
        /// <returns>The luminance.</returns>
        public static double LToY(double L)
        {
            if (L <= 8)
                return RefY * L / Kappa;
            else
                return RefY * Math.Pow((L + 16) / 116, 3);
        }

        public static IList<double> XyzToLuv(IList<double> tuple)
        {
            double X = tuple[0];
            double Y = tuple[1];
            double Z = tuple[2];

            double varU = (4 * X) / (X + (15 * Y) + (3 * Z));
            double varV = (9 * Y) / (X + (15 * Y) + (3 * Z));

            double L = YToL(Y);

            if (L == 0)
            {
                return new double[] { 0, 0, 0 };
            }

            var U = 13 * L * (varU - RefU);
            var V = 13 * L * (varV - RefV);

            return new Double[] { L, U, V };
        }

        public static IList<double> LuvToXyz(IList<double> tuple)
        {
            double L = tuple[0];
            double U = tuple[1];
            double V = tuple[2];

            if (L == 0)
            {
                return new double[] { 0, 0, 0 };
            }

            double varU = U / (13 * L) + RefU;
            double varV = V / (13 * L) + RefV;

            double Y = LToY(L);
            double X = 0 - (9 * Y * varU) / ((varU - 4) * varV - varU * varV);
            double Z = (9 * Y - (15 * varV * Y) - (varV * X)) / (3 * varV);

            return new double[] { X, Y, Z };
        }

        public static IList<double> LuvToLch(IList<double> tuple)
        {
            double L = tuple[0];
            double U = tuple[1];
            double V = tuple[2];

            double C = Math.Pow(Math.Pow(U, 2) + Math.Pow(V, 2), 0.5);
            double Hrad = Math.Atan2(V, U);

            double H = Hrad * 180.0 / Math.PI;

            if (H < 0)
            {
                H = 360 + H;
            }

            return new double[] { L, C, H };
        }

        public static IList<double> LchToLuv(IList<double> tuple)
        {
            double L = tuple[0];
            double C = tuple[1];
            double H = tuple[2];

            double Hrad = H / 360.0 * 2 * Math.PI;
            double U = Math.Cos(Hrad) * C;
            double V = Math.Sin(Hrad) * C;

            return new Double[] { L, U, V };
        }

        public static IList<double> HsluvToLch(IList<double> tuple)
        {
            double H = tuple[0];
            double S = tuple[1];
            double L = tuple[2];

            if (L > 99.9999999)
            {
                return new Double[] { 100, 0, H };
            }

            if (L < 0.00000001)
            {
                return new Double[] { 0, 0, H };
            }

            double max = MaxChromaForLH(L, H);
            double C = max / 100 * S;

            return new double[] { L, C, H };
        }

        public static IList<double> LchToHsluv(IList<double> tuple)
        {
            double L = tuple[0];
            double C = tuple[1];
            double H = tuple[2];

            if (L > 99.9999999)
            {
                return new Double[] { H, 0, 100 };
            }

            if (L < 0.00000001)
            {
                return new Double[] { H, 0, 0 };
            }

            double max = MaxChromaForLH(L, H);
            double S = C / max * 100;

            return new double[] { H, S, L };
        }

        public static IList<double> HpluvToLch(IList<double> tuple)
        {
            double H = tuple[0];
            double S = tuple[1];
            double L = tuple[2];

            if (L > 99.9999999)
            {
                return new Double[] { 100, 0, H };
            }

            if (L < 0.00000001)
            {
                return new Double[] { 0, 0, H };
            }

            double max = MaxSafeChromaForL(L);
            double C = max / 100 * S;

            return new double[] { L, C, H };
        }

        public static IList<double> LchToHpluv(IList<double> tuple)
        {
            double L = tuple[0];
            double C = tuple[1];
            double H = tuple[2];

            if (L > 99.9999999)
            {
                return new Double[] { H, 0, 100 };
            }

            if (L < 0.00000001)
            {
                return new Double[] { H, 0, 0 };
            }

            double max = MaxSafeChromaForL(L);
            double S = C / max * 100;

            return new double[] { H, S, L };
        }

        public static IList<double> LchToRgb(IList<double> tuple)
        {
            return XyzToRgb(LuvToXyz(LchToLuv(tuple)));
        }

        public static IList<double> RgbToLch(IList<double> tuple)
        {
            return LuvToLch(XyzToLuv(RgbToXyz(tuple)));
        }

        // Rgb <-> Hsluv

        public static IList<double> HsluvToRgb(IList<double> tuple)
        {
            return LchToRgb(HsluvToLch(tuple));
        }

        public static IList<double> RgbToHsluv(IList<double> tuple)
        {
            return LchToHsluv(RgbToLch(tuple));
        }

        public static IList<double> HpluvToRgb(IList<double> tuple)
        {
            return LchToRgb(HpluvToLch(tuple));
        }

        public static IList<double> RgbToHpluv(IList<double> tuple)
        {
            return LchToHpluv(RgbToLch(tuple));
        }
    }
}

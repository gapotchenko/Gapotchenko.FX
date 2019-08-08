using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Drawing
{
    /// <summary>
    /// Color metrics functions.
    /// </summary>
    public static class ColorMetrics
    {
        /// <summary>
        /// Calculates contrast ratio.
        /// </summary>
        /// <param name="a">Luminance A.</param>
        /// <param name="b">Luminance B.</param>
        /// <returns>The contrast ratio value ranging from 1 to 21.</returns>
        static double _ContrastRatio(double a, double b)
        {
            if (b > a)
            {
                var t = a;
                a = b;
                b = t;
            }

            return (a + 0.05) / (b + 0.05);
        }

        /// <summary>
        /// Calculates similarity factor between two colors.
        /// The returned value lays between 0.0 and 1.0 inclusively.
        /// When the given colors are equal, the returned similarity factor is 1.0.
        /// When the given colors are completely different (like black and white), the returned similarity factor is 0.0.
        /// Specifics of human visual perception are taken into account in accordance to the following standards and approaches:
        /// ISO-9241-3, ANSI-HFES-100-1988, CIELUV.
        /// </summary>
        /// <param name="a">Color A.</param>
        /// <param name="b">Color B.</param>
        /// <returns>The similarity factor ranging from 0 to 1.</returns>
        public static double Similarity(Color a, Color b)
        {
            // The function contains elements of an original research by Oleksiy Gapotchenko 2017.

            var hslA = Hsluv.RgbToHsluv(new double[] { a.R / 255.0, a.G / 255.0, a.B / 255.0 });
            var hslB = Hsluv.RgbToHsluv(new double[] { b.R / 255.0, b.G / 255.0, b.B / 255.0 });

            // -------------------------------------------------------------------------

            double hueDistanceA = Math.Abs(hslA[0] - hslB[0]);
            double hueDistanceB = 360 - hueDistanceA;
            // hueDistance Є [0; 180]
            double hueDistance = Math.Min(hueDistanceA, hueDistanceB);

            // Normalize hue distance so that hueDistance Є [0; 1].
            hueDistance = hueDistance / 180.0;

            // -------------------------------------------------------------------------

            // contrastRatio Є [1; 21]
            double contrastRatio = _ContrastRatio(Hsluv.LToY(hslA[2]), Hsluv.LToY(hslB[2]));

            // Normalize contrast ratio so that contrastRatio Є [0; 1].
            contrastRatio = (contrastRatio - 1.0) / 20.0;

            // -------------------------------------------------------------------------

            // saturation Є [0; 1]
            double saturation = hslA[1] * hslB[1] / 10000.0;

            // -------------------------------------------------------------------------

            double difference = contrastRatio * (1.0 - saturation) + hueDistance * saturation * 0.15;
            double s = 1.0 - difference;

            return s;
        }
    }
}

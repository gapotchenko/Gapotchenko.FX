using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gapotchenko.FX.Runtime.InteropServices
{
    static class CodeSafetyStrategy
    {
        static CodeSafetyStrategy()
        {
            UnsafeCodeAllowed = AppDomain.CurrentDomain.IsFullyTrusted;
        }

        public static bool UnsafeCodeAllowed { get; }

        static LazyEvaluation<bool> _IsUnsafeCodeRecommended = LazyEvaluation.Create(_IsUnsafeCodeRecommendedCore);

        public static bool IsUnsafeCodeRecommended => _IsUnsafeCodeRecommended.Value;

        static bool _IsUnsafeCodeRecommendedCore()
        {
            if (!UnsafeCodeAllowed)
                return false;

            if (AppDomain.CurrentDomain.GetData(".appDomain") != null)
            {
                // Code is running within a web server application.
                return false;
            }

            return true;
        }
    }
}

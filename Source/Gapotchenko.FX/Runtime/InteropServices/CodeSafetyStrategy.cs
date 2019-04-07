using Gapotchenko.FX.Threading;
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

        /// <summary>
        /// Indicates whether unsafe code is allowed in the current execution context.
        /// </summary>
        public static bool UnsafeCodeAllowed { get; }

        static EvaluateOnce<bool> _UnsafeCodeRecommended = EvaluateOnce.Create(_IsUnsafeCodeRecommendedCore);

        /// <summary>
        /// Indicates whether unsafe code is recommended in the current execution context.
        /// </summary>
        public static bool UnsafeCodeRecommended => _UnsafeCodeRecommended.Value;

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

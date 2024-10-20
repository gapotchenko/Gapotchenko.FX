using Gapotchenko.FX.Threading;

namespace Gapotchenko.FX.Runtime.InteropServices;

/// <summary>
/// Provides information on code safety strategy.
/// </summary>
public static class CodeSafetyStrategy
{
    static CodeSafetyStrategy()
    {
        UnsafeCodeAllowed = AppDomain.CurrentDomain.IsFullyTrusted;
    }

    /// <summary>
    /// Indicates whether unsafe code is allowed in the current execution context.
    /// </summary>
    public static bool UnsafeCodeAllowed { get; }

    /// <summary>
    /// Indicates whether unsafe code is recommended in the current execution context.
    /// </summary>
    public static bool UnsafeCodeRecommended => m_UnsafeCodeRecommended.Value;

    static EvaluateOnce<bool> m_UnsafeCodeRecommended = new(IsUnsafeCodeRecommendedCore);

    static bool IsUnsafeCodeRecommendedCore()
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

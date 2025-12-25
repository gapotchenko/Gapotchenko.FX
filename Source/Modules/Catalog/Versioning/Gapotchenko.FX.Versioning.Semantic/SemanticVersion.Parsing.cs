// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Threading;
using Gapotchenko.FX.Versioning.Properties;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <summary>
    /// Converts the string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="input">A string that contains a semantic version to convert.</param>
    /// <returns>An object that is equivalent to the version string specified in the input parameter.</returns>
    /// <exception cref="FormatException"><paramref name="input"/> string has an invalid format.</exception>
    [return: NotNullIfNotNull(nameof(input))]
    public static SemanticVersion? Parse(string? input) =>
        input is null ?
            null :
            new(ParseCore(input));

    /// <summary>
    /// Tries to convert the string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> object,
    /// and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="input">A string that contains a semantic version to convert.</param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="SemanticVersion"/> equivalent of the version string specified in the input, if the conversion succeeded;
    /// or null if the conversion failed.
    /// </param>
    /// <returns><c>true</c> if the input parameter was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? input, [NotNullWhen(true)] out SemanticVersion? result) =>
        (result = TryParse(input)) is not null;

    /// <summary>
    /// Tries to convert the string representation of a semantic version to an equivalent <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="input">A string that contains a semantic version to convert.</param>
    /// <returns>An object that is equivalent to the version string specified in the input parameter if conversion was successful; otherwise, null.</returns>
    public static SemanticVersion? TryParse(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return null;
        else
            return Create(TryParseCore(input));
    }

    static Model ParseCore(string input) =>
         Parser.TryParseVersion(input) ??
         throw new FormatException(Resources.SemanticVersionHasInvalidFormat);

    static Model? TryParseCore(string input) => Parser.TryParseVersion(input);

    /// <remarks>
    /// The parser implementation is based on regular expressions provided in the specification:
    /// https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string
    /// </remarks>
    static partial class Parser
    {
        #region Version

        public static Model? TryParseVersion(string input)
        {
            if (input.Length < 5 /* 0.0.0 */ ||
                !char.IsDigit(input[0]))
            {
                // Fast discard.
                return null;
            }

            var match = GetVersionRegex().Match(input);
            if (!match.Success)
                return null;

            const NumberStyles numberStyle = NumberStyles.None;
            var numberFormatProvider = NumberFormatInfo.InvariantInfo;

            if (!int.TryParse(match.Groups[MajorRegexGroupName].Value, numberStyle, numberFormatProvider, out int major))
                return null;
            if (!int.TryParse(match.Groups[MinorRegexGroupName].Value, numberStyle, numberFormatProvider, out int minor))
                return null;
            if (!int.TryParse(match.Groups[PatchRegexGroupName].Value, numberStyle, numberFormatProvider, out int patch))
                return null;

            return
                new Model
                {
                    Major = major,
                    Minor = minor,
                    Patch = patch,
                    Prerelease = FX.Empty.Nullify(match.Groups[PrereleaseRegexGroupName].Value),
                    Build = FX.Empty.Nullify(match.Groups[BuildRegexGroupName].Value)
                };
        }

#if NET7_0_OR_GREATER
        [GeneratedRegex(VersionRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
        private static partial Regex GetVersionRegex();
#else
        static Regex GetVersionRegex() => m_VersionRegex.Value;

        static EvaluateOnce<Regex> m_VersionRegex = new(
            () => new(VersionRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
            m_Lock);
#endif

        const string VersionRegexPattern = $@"^(?<{MajorRegexGroupName}>0|[1-9]\d*)\.(?<{MinorRegexGroupName}>0|[1-9]\d*)\.(?<{PatchRegexGroupName}>0|[1-9]\d*){LabelRegexText}$";

        const string MajorRegexGroupName = "major";
        const string MinorRegexGroupName = "minor";
        const string PatchRegexGroupName = "patch";

        #endregion

        #region Label component of the version

        public static bool TryParseLabel(string label, out string? prerelease, out string? build)
        {
            var match = GetLabelRegex().Match(label);
            if (match.Success)
            {
                prerelease = FX.Empty.Nullify(match.Groups[PrereleaseRegexGroupName].Value);
                build = FX.Empty.Nullify(match.Groups[BuildRegexGroupName].Value);
                return true;
            }
            else
            {
                prerelease = default;
                build = default;
                return false;
            }
        }

#if NET7_0_OR_GREATER
        [GeneratedRegex(LabelRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
        private static partial Regex GetLabelRegex();
#else
        static Regex GetLabelRegex() => m_LabelRegex.Value;

        static EvaluateOnce<Regex> m_LabelRegex = new(
            () => new(LabelRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
            m_Lock);
#endif

        const string LabelRegexPattern = $"^{LabelRegexText}$";

        const string LabelRegexText = $@"(-(?<{PrereleaseRegexGroupName}>{PrereleaseRegexText}))?(\+(?<{BuildRegexGroupName}>{BuildRegexText}))?";

        const string PrereleaseRegexGroupName = "prerelease";
        const string BuildRegexGroupName = "build";

        #endregion

        #region Build metadata component of the label

        public static bool IsValidBuild(string? value) => value is null || GetBuildRegex().IsMatch(value);

#if NET7_0_OR_GREATER
        [GeneratedRegex(BuildRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
        private static partial Regex GetBuildRegex();
#else
        static Regex GetBuildRegex() => m_BuildRegex.Value;

        static readonly EvaluateOnce<Regex> m_BuildRegex = new(
            () => new(BuildRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
            m_Lock);
#endif

        const string BuildRegexPattern = $"^{BuildRegexText}$";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        const string BuildRegexText = @"[0-9a-zA-Z-]+(\.[0-9a-zA-Z-]+)*";

        #endregion

        #region Prerelease component of the label 

        public static bool IsValidPrerelease(string? value) => value is null || GetPrereleaseRegex().IsMatch(value);

#if NET7_0_OR_GREATER
        [GeneratedRegex(PrereleaseRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
        private static partial Regex GetPrereleaseRegex();
#else
        static Regex GetPrereleaseRegex() => m_PrereleaseRegex.Value;

        static readonly EvaluateOnce<Regex> m_PrereleaseRegex = new(
            () => new(PrereleaseRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
            m_Lock);
#endif

        const string PrereleaseRegexPattern = $"^{PrereleaseRegexText}$";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        const string PrereleaseRegexText = @"(0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*";

        #endregion

#if !NET7_0_OR_GREATER
        static readonly object m_Lock = new();
#endif
    }
}

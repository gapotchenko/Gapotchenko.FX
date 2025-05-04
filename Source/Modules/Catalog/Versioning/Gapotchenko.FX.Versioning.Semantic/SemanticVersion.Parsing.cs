// Gapotchenko.FX
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
    /// Converts the string representation of a version number to an equivalent <see cref="SemanticVersion"/> object.
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
    /// Tries to convert the string representation of a version string to an equivalent <see cref="SemanticVersion"/> object,
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
    /// Tries to convert the string representation of a version string to an equivalent <see cref="SemanticVersion"/> object.
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

    static partial class Parser
    {
        public static bool IsValidLabel(string? s) => s is null || GetLabelRegex().IsMatch(s);

#if NET7_0_OR_GREATER
        [GeneratedRegex(LabelRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
        private static partial Regex GetLabelRegex();
#else
        static Regex GetLabelRegex() => m_LabelRegex.Value;

        static readonly EvaluateOnce<Regex> m_LabelRegex = new(
            () => new(LabelRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
            m_Lock);
#endif

        const string LabelRegexPattern = $"^{LabelRegexText}$";

        public static bool TryParseLabels(string labels, out string? preReleaseLabel, out string? buildLabel)
        {
            var match = GetLabelsRegex().Match(labels);
            if (match.Success)
            {
                preReleaseLabel = FX.Empty.Nullify(match.Groups["prl"].Value);
                buildLabel = FX.Empty.Nullify(match.Groups["bl"].Value);
                return true;
            }
            else
            {
                preReleaseLabel = default;
                buildLabel = default;
                return false;
            }
        }

#if NET7_0_OR_GREATER
        [GeneratedRegex(LabelsRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture)]
        private static partial Regex GetLabelsRegex();
#else
        static Regex GetLabelsRegex() => m_LabelsRegex.Value;

        static EvaluateOnce<Regex> m_LabelsRegex = new(
            () => new(LabelsRegexPattern, RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture),
            m_Lock);
#endif

        const string LabelsRegexPattern = $@"^(-?(?<prl>{LabelRegexText}))?(\+(?<bl>{LabelRegexText}))?$";

        public static Model? TryParseVersion(string input)
        {
            var match = GetVersionRegex().Match(input);
            if (!match.Success)
                return null;

            var majorGroup = match.Groups["ma"];
            if (!majorGroup.Success)
                return null;
            if (!int.TryParse(majorGroup.Value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out int major))
                return null;

            int minor = 0;
            var minorGroup = match.Groups["mi"];
            if (minorGroup.Success)
            {
                if (!int.TryParse(minorGroup.Value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out minor))
                    return null;
            }

            int patch = 0;
            var patchrGroup = match.Groups["p"];
            if (patchrGroup.Success)
            {
                if (!int.TryParse(patchrGroup.Value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out patch))
                    return null;
            }

            return
                new Model
                {
                    Major = major,
                    Minor = minor,
                    Patch = patch,
                    PreReleaseLabel = FX.Empty.Nullify(match.Groups["prl"].Value),
                    BuildLabel = FX.Empty.Nullify(match.Groups["bl"].Value)
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

        const string VersionRegexPattern = $@"^(?<ma>\d+)(\.(?<mi>\d+))?(\.(?<p>\d+))?(-(?<prl>{LabelRegexText}))?(\+(?<bl>{LabelRegexText}))?$";

#if !NET7_0_OR_GREATER
        static readonly object m_Lock = new();
#endif

        const string LabelRegexText = @"[0-9A-Za-z][0-9A-Za-z\-\.]+";
    }
}

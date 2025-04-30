// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Versioning;

partial class SemanticVersion
{
    /// <summary>
    /// Converts the string representation of a version number to an equivalent <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="input">A string that contains a semantic version to convert.</param>
    /// <returns>An object that is equivalent to the version string specified in the input parameter.</returns>
    public static SemanticVersion Parse(string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        var version = new SemanticVersion();
        if (version.TryParseCore(input))
            return version;
        else
            throw new FormatException("Semantic version has an invalid format.");
    }

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
    public static bool TryParse(string? input, out SemanticVersion result)
    {
        var value = TryParse(input);
        result = value;
        return value is not null;
    }

    /// <summary>
    /// Tries to convert the string representation of a version string to an equivalent <see cref="SemanticVersion"/> object.
    /// </summary>
    /// <param name="input">A string that contains a semantic version to convert.</param>
    /// <returns>An object that is equivalent to the version string specified in the input parameter if conversion was successful; otherwise, null.</returns>
    public static SemanticVersion? TryParse(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        var version = new SemanticVersion();
        if (version.TryParseCore(input))
            return version;
        else
            return null;
    }

    bool TryParseCore(string version) =>
        Parser.TryParseVersion(
            version,
            out m_Major, out m_Minor, out m_Patch,
            out m_PreReleaseLabel, out m_BuildLabel);

    static partial class Parser
    {
        public static bool IsValidLabel(string s) => GetLabelRegex().IsMatch(s);

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
                preReleaseLabel = Empty.Nullify(match.Groups["prl"].Value);
                buildLabel = Empty.Nullify(match.Groups["bl"].Value);
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

        public static bool TryParseVersion(
            string version,
            out int major, out int minor, out int patch,
            out string? preReleaseLabel, out string? buildLabel)
        {
            major = default;
            minor = default;
            patch = default;
            preReleaseLabel = default;
            buildLabel = default;

            var match = GetVersionRegex().Match(version);
            if (!match.Success)
                return false;

            var majorGroup = match.Groups["ma"];
            if (!majorGroup.Success)
                return false;
            if (!int.TryParse(majorGroup.Value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out major))
                return false;

            var minorGroup = match.Groups["mi"];
            if (minorGroup.Success)
            {
                if (!int.TryParse(minorGroup.Value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out minor))
                    return false;
            }

            var patchrGroup = match.Groups["p"];
            if (patchrGroup.Success)
            {
                if (!int.TryParse(patchrGroup.Value, NumberStyles.None, NumberFormatInfo.InvariantInfo, out patch))
                    return false;
            }

            preReleaseLabel = Empty.Nullify(match.Groups["prl"].Value);
            buildLabel = Empty.Nullify(match.Groups["bl"].Value);

            return true;
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

        const string LabelRegexText = @"[0-9A-Za-z][0-9A-Za-z\-\.]*";
    }
}

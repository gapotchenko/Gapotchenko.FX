using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

static class RegexExtensions
{
    public static IEnumerable<Match> EnumerateMatches(this Regex regex, string input)
    {
        if (regex == null)
            throw new ArgumentNullException(nameof(regex));

        return _SelfAndNextMatches(regex.Match(input));
    }

    static IEnumerable<Match> _SelfAndNextMatches(this Match match)
    {
        while (match.Success)
        {
            yield return match;
            match = match.NextMatch();
        }
    }
}

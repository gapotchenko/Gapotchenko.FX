using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

static class RegexExtensions
{
    public static IEnumerable<Match> EnumerateMatchesLinq(this Regex regex, string input)
    {
        if (regex == null)
            throw new ArgumentNullException(nameof(regex));

        return ThisAndNextMatches(regex.Match(input));

        static IEnumerable<Match> ThisAndNextMatches(Match match)
        {
            while (match.Success)
            {
                yield return match;
                match = match.NextMatch();
            }
        }
    }
}

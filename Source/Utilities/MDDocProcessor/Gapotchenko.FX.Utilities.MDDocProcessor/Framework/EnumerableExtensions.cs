using Gapotchenko.FX.Linq;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Utilities.MDDocProcessor.Framework;

static partial class EnumerableExtensions
{
    public static IEnumerable<T> OrderNaturallyBy<T>(this IEnumerable<T> source, Func<T, string> keySelector)
    {
        source = source.Memoize();

        if (!source.Any())
            return [];

        int totalWidth = source.Select(value => keySelector(value).Length).Max();

        return source
            .Select(value =>
                (
                    Value: value,
                    Key: NumberRegex().Replace(
                        keySelector(value),
                        m => m.Value.PadLeft(totalWidth, PaddingChar(m.Value)))
                ))
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

        static char PaddingChar(string s) => char.IsDigit(s[0]) ? ' ' : char.MaxValue;
    }

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex NumberRegex();
}

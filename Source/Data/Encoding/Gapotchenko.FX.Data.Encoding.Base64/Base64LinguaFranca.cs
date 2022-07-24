namespace Gapotchenko.FX.Data.Encoding;

static class Base64LinguaFranca
{
    /// <summary>
    /// Gets a common alphabet for <see cref="Base64"/> and <see cref="Base64Url"/> encoding variants.
    /// </summary>
    public static TextDataEncodingAlphabet Alphabet { get; } =
        new TextDataEncodingAlphabet(
            Base64.Symbols,
            true,
            new Dictionary<char, string>
            {
                ['+'] = "-",
                ['/'] = "_"
            });
}

namespace Gapotchenko.FX.Data.Encoding;

partial class Base16
{
    static class FastDecoder
    {
        public static bool SupportsOptions(DataEncodingOptions options) =>
            (options & (DataEncodingOptions.Pure | DataEncodingOptions.Indent | DataEncodingOptions.Relax)) == DataEncodingOptions.Pure;

        public static byte[]? GetBytes(ReadOnlySpan<char> input, bool throwOnError)
        {
            int length = input.Length;
            if ((length & 0x1) != 0)
            {
                if (throwOnError)
                    throw new FormatException($"{Name}-encoded string contains an odd number of characters.");
                return null;
            }

            var result = new byte[length >> 1];

            for (int i = 0, si = 0; i < result.Length; i++)
            {
                byte h = TryParseNibble(input[si++]);
                byte l = TryParseNibble(input[si++]);

                if ((h | l) == byte.MaxValue)
                {
                    if (throwOnError)
                        throw new FormatException($"Encountered a non-{Name} character.");
                    return null;
                }

                result[i] = (byte)((h << 4) | l);
            }

            return result;

            static byte TryParseNibble(char input) =>
                input switch
                {
                    >= '0' and <= '9' => (byte)(input - '0'),
                    >= 'a' and <= 'f' => (byte)(input - 'a' + 10),
                    >= 'A' and <= 'F' => (byte)(input - 'A' + 10),
                    _ => byte.MaxValue
                };
        }
    }
}

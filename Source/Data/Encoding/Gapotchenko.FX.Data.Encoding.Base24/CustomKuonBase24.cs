namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Customizable Kuon Base24 encoding.
/// </summary>
public class CustomKuonBase24 : GenericKuonBase24
{
    /// <summary>
    /// Initializes a new instance of <see cref="CustomKuonBase24"/> class with the specified case-insensitive alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    public CustomKuonBase24(string alphabet) :
        this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet)), false))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CustomKuonBase24"/> class with the specified alphabet and padding char.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="paddingChar">The padding char.</param>
    public CustomKuonBase24(TextDataEncodingAlphabet alphabet, char paddingChar = '=') :
        base(alphabet, paddingChar)
    {
    }
}

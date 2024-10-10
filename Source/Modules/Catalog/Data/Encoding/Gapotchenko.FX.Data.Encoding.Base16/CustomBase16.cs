namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Provides implementation of a customizable Base16 encoding.
/// </summary>
public class CustomBase16 : GenericBase16
{
    /// <summary>
    /// Initializes a new instance of <see cref="CustomBase16"/> class with the specified case-insensitive alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    public CustomBase16(string alphabet) :
        this(
            new TextDataEncodingAlphabet(
                alphabet ?? throw new ArgumentNullException(nameof(alphabet)),
                false))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CustomBase16"/> class with the specified alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    public CustomBase16(TextDataEncodingAlphabet alphabet) :
        base(alphabet)
    {
    }
}

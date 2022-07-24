namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Customizable Crockford Base 32 encoding.
/// </summary>
public class CustomCrockfordBase32 : GenericCrockfordBase32
{
    /// <summary>
    /// Initializes a new instance of <see cref="CustomCrockfordBase32"/> class with the specified case-insensitive alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    public CustomCrockfordBase32(string alphabet) :
        this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet)), false))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CustomCrockfordBase32"/> class with the specified alphabet and padding character.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="paddingChar">The padding character.</param>
    public CustomCrockfordBase32(TextDataEncodingAlphabet alphabet, char paddingChar = '=') :
        base(alphabet, paddingChar)
    {
    }
}

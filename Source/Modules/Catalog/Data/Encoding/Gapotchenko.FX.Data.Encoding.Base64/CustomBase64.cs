// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// Customizable Base64 encoding.
/// </summary>
public class CustomBase64 : GenericBase64
{
    /// <summary>
    /// Initializes a new instance of <see cref="CustomBase64"/> class with the specified case-sensitive alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    public CustomBase64(string alphabet) :
        this(
            new TextDataEncodingAlphabet(
                alphabet ?? throw new ArgumentNullException(nameof(alphabet)),
                true))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CustomBase64"/> class with the specified alphabet and padding character.
    /// </summary>
    /// <param name="alphabet">The alphabet.</param>
    /// <param name="paddingChar">The padding character.</param>
    public CustomBase64(TextDataEncodingAlphabet alphabet, char paddingChar = '=') :
        base(alphabet, paddingChar)
    {
    }
}

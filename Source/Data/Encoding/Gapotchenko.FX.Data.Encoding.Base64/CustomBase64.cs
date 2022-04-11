﻿using System;

namespace Gapotchenko.FX.Data.Encoding
{
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
            this(new TextDataEncodingAlphabet(alphabet ?? throw new ArgumentNullException(nameof(alphabet)), true))
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CustomBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomBase64(TextDataEncodingAlphabet alphabet) :
            base(alphabet)
        {
        }
    }
}
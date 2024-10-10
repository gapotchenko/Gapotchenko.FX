// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Data.Encoding;

/// <summary>
/// A unifying interface for all possible Crockford Base 32 encoding implementations.
/// </summary>
[CLSCompliant(false)]
public interface ICrockfordBase32 : IBase32, INumericTextDataEncoding
{
}

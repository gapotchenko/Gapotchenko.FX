// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

partial struct VfsSearchExpression
{
    /// <summary>
    /// Implements pattern matching algorithm equivalent to <c>.+</c> regular expression.
    /// </summary>
    sealed class AnyPlusImpl : IImpl
    {
        public static AnyPlusImpl Instance { get; } = new();

        public bool IsMatch(ReadOnlySpan<char> input) => !input.IsEmpty;
    }
}

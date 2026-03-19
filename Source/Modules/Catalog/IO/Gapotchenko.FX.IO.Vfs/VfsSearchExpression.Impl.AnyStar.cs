// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.IO.Vfs;

partial struct VfsSearchExpression
{
    /// <summary>
    /// Implements pattern matching algorithm equivalent to <c>.*</c> regular expression.
    /// </summary>
    sealed class AnyStarImpl : IImpl
    {
        public static AnyStarImpl Instance { get; } = new();

        public bool IsMatch(ReadOnlySpan<char> input) => true;
    }
}

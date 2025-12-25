// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

partial struct VfsSearchExpression
{
    sealed class SimpleImpl(string pattern, VfsSearchExpressionOptions options) : IImpl
    {
        public bool IsMatch(ReadOnlySpan<char> input)
        {
            return MatchPattern(
                pattern.AsSpan(),
                input,
                (options & VfsSearchExpressionOptions.IgnoreCase) != 0,
                false);
        }
    }
}

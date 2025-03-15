// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Vfs.Kits;

/// <summary>
/// Provides validation facilities for a virtual file system.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class VfsValidationKit
{
    /// <summary>
    /// Provides facilities for validation of function arguments.
    /// </summary>
    [StackTraceHidden]
    public static class Arguments
    {
        /// <summary>
        /// Validates the specified path argument value.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is empty.</exception>
        public static void ValidatePath(
            [NotNull] string? path,
            [CallerArgumentExpression(nameof(path))] string? argumentName = null)
        {
            if (path is null)
                throw new ArgumentNullException(argumentName);
            if (path.Length == 0)
                throw new ArgumentException(Resources.PathIsEmpty, argumentName);
        }

        /// <summary>
        /// Validates the specified search pattern argument value.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
        public static void ValidateSearchPattern(
            [NotNull] string? searchPattern,
            [CallerArgumentExpression(nameof(searchPattern))] string? argumentName = null)
        {
            if (searchPattern is null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Validates the specified <see cref="SearchOption"/> argument value.
        /// </summary>
        /// <param name="searchOption">The <see cref="SearchOption"/> value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="searchOption"/> value is out of legal range.</exception>
        public static void ValidateSearchOption(
            SearchOption searchOption,
            [CallerArgumentExpression(nameof(searchOption))] string? argumentName = null)
        {
            if (searchOption is not (SearchOption.TopDirectoryOnly or SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException(argumentName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="FileMode"/> argument value.
        /// </summary>
        /// <param name="fileMode">The <see cref="FileMode"/> value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileMode"/> value is out of legal range.</exception>
        public static void ValidateFileMode(
            FileMode fileMode,
            [CallerArgumentExpression(nameof(fileMode))] string? argumentName = null)
        {
            if (fileMode is not (FileMode.Append or FileMode.Create or FileMode.CreateNew or FileMode.Open or FileMode.OpenOrCreate or FileMode.Truncate))
                throw new ArgumentOutOfRangeException(argumentName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="FileAccess"/> argument value.
        /// </summary>
        /// <param name="fileAccess">The <see cref="FileAccess"/> value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileAccess"/> value is out of legal range.</exception>
        public static void ValidateFileAccess(
            FileAccess fileAccess,
            [CallerArgumentExpression(nameof(fileAccess))] string? argumentName = null)
        {
            if ((fileAccess & ~FileAccess.ReadWrite) != 0)
                throw new ArgumentOutOfRangeException(argumentName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="FileShare"/> argument value.
        /// </summary>
        /// <param name="fileShare">The <see cref="FileShare"/> value.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileShare"/> value is out of legal range.</exception>
        public static void ValidateFileShare(
            FileShare fileShare,
            [CallerArgumentExpression(nameof(fileShare))] string? argumentName = null)
        {
            if ((fileShare & ~(FileShare.ReadWrite | FileShare.Delete)) != 0)
                throw new ArgumentOutOfRangeException(argumentName, Resources.EnumValueIsOutOfLegalRange);
        }
    }
}

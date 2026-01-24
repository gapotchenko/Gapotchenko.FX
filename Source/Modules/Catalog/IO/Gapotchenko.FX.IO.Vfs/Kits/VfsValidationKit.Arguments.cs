// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.IO.Vfs.Kits;

partial class VfsValidationKit
{
    /// <summary>
    /// Provides function argument validation facilities.
    /// </summary>
    [StackTraceHidden]
    public static class Arguments
    {
        /// <summary>
        /// Validates the specified path argument.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="path"/> is empty.</exception>
        public static void ValidatePath(
            [NotNull] string? path,
            [CallerArgumentExpression(nameof(path))] string? parameterName = null)
        {
            ArgumentNullException.ThrowIfNull(path, parameterName);
            if (path.Length == 0)
                throw new ArgumentException(Resources.PathIsEmpty, parameterName);
        }

        /// <summary>
        /// Validates the specified search pattern argument.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
        public static void ValidateSearchPattern(
            [NotNull] string? searchPattern,
            [CallerArgumentExpression(nameof(searchPattern))] string? parameterName = null)
        {
            ArgumentNullException.ThrowIfNull(searchPattern, parameterName);
        }

        /// <summary>
        /// Validates the specified <see cref="SearchOption"/> argument.
        /// </summary>
        /// <param name="searchOption">The <see cref="SearchOption"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="searchOption"/> value is out of legal range.</exception>
        public static void ValidateSearchOption(
            SearchOption searchOption,
            [CallerArgumentExpression(nameof(searchOption))] string? parameterName = null)
        {
            if (searchOption is not (SearchOption.TopDirectoryOnly or SearchOption.AllDirectories))
                throw new ArgumentOutOfRangeException(parameterName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="EnumerationOptions"/> argument.
        /// </summary>
        /// <param name="enumerationOptions">The <see cref="EnumerationOptions"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="enumerationOptions"/> value is out of legal range.</exception>
        public static void ValidateEnumerationOptions(
            EnumerationOptions enumerationOptions,
            [CallerArgumentExpression(nameof(enumerationOptions))] string? parameterName = null)
        {
            ArgumentNullException.ThrowIfNull(enumerationOptions, parameterName);
        }

        /// <summary>
        /// Validates the specified <see cref="FileMode"/> and <see cref="FileAccess"/> argument.
        /// </summary>
        /// <param name="mode">The <see cref="FileMode"/> value.</param>
        /// <param name="access">The <see cref="FileAccess"/> value.</param>
        /// <param name="modeParameterName">The <see cref="FileMode"/> parameter name.</param>
        /// <param name="accessParameterName">The <see cref="FileAccess"/> parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode"/> value is out of legal range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="access"/> value is out of legal range.</exception>
        public static void ValidateFileModeAndAccess(
            FileMode mode,
            FileAccess access,
            [CallerArgumentExpression(nameof(mode))] string? modeParameterName = null,
            [CallerArgumentExpression(nameof(access))] string? accessParameterName = null)
        {
            ValidateFileMode(mode, modeParameterName);
            ValidateFileAccess(access, accessParameterName);

            // TODO: validate combinations
        }

        /// <summary>
        /// Validates the specified <see cref="FileMode"/> argument.
        /// </summary>
        /// <param name="mode">The <see cref="FileMode"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode"/> value is out of legal range.</exception>
        public static void ValidateFileMode(
            FileMode mode,
            [CallerArgumentExpression(nameof(mode))] string? parameterName = null)
        {
            if (mode is not (FileMode.Append or FileMode.Create or FileMode.CreateNew or FileMode.Open or FileMode.OpenOrCreate or FileMode.Truncate))
                throw new ArgumentOutOfRangeException(parameterName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="FileAccess"/> argument.
        /// </summary>
        /// <param name="access">The <see cref="FileAccess"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="access"/> value is out of legal range.</exception>
        public static void ValidateFileAccess(
            FileAccess access,
            [CallerArgumentExpression(nameof(access))] string? parameterName = null)
        {
            if (access == 0 || (access & ~FileAccess.ReadWrite) != 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="FileShare"/> argument.
        /// </summary>
        /// <param name="share">The <see cref="FileShare"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="share"/> value is out of legal range.</exception>
        public static void ValidateFileShare(
            FileShare share,
            [CallerArgumentExpression(nameof(share))] string? parameterName = null)
        {
            if ((share & ~(FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable)) != 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="VfsCopyOptions"/> argument.
        /// </summary>
        /// <param name="options">The <see cref="VfsCopyOptions"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> value is out of legal range.</exception>
        public static void ValidateCopyOptions(
            VfsCopyOptions options,
            [CallerArgumentExpression(nameof(options))] string? parameterName = null)
        {
            const VfsCopyOptions knownOptions = VfsCopyOptions.Archive;
            if ((options & ~knownOptions) != 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.EnumValueIsOutOfLegalRange);
        }

        /// <summary>
        /// Validates the specified <see cref="VfsMoveOptions"/> argument.
        /// </summary>
        /// <param name="options">The <see cref="VfsMoveOptions"/> value.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> value is out of legal range.</exception>
        public static void ValidateMoveOptions(
            VfsMoveOptions options,
            [CallerArgumentExpression(nameof(options))] string? parameterName = null)
        {
            const VfsMoveOptions knownOptions = VfsMoveOptions.None;
            if ((options & ~knownOptions) != 0)
                throw new ArgumentOutOfRangeException(parameterName, Resources.EnumValueIsOutOfLegalRange);
        }
    }
}

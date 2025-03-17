// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if DEVELOPMENT_PROTOTYPE

// This development prototype demonstrates a possible approach for
// cross-VFS interactions that can be used to implement optimized IO
// operations.

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Defines the root interface of an <see cref="IFileSystemView"/> aspect.
/// </summary>
/// <remarks>
/// The aspect is an add-on functionality
/// that can be exposed by a particular file system view implementation.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IFileSystemViewAspect
{
}

/// <summary>
/// Provides an <see cref="IFileSystemViewAspect"/> instance.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IFileSystemViewAspectProvider
{
    /// <summary>
    /// Gets an aspect instance of the specified type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the aspect to get.</typeparam>
    /// <returns>The <typeparamref name="T"/> instance, or <see langword="null"/> if the aspect of the requested type is unavailable.</returns>
    T? GetAspect<T>() where T : class, IFileSystemViewAspect;
}

/// <summary>
/// The <see cref="IFileSystemView"/> aspect that defines the ability
/// to copy file-system entries from other <see cref="IFileSystemView"/> implementations.
/// </summary>
public interface IFileSystemViewCopyFromAspect : IFileSystemViewAspect
{
    // Can throw NotSupportedException for a fallback logic to kick in.
    void CopyFileFrom(IReadOnlyFileSystemView sourceView, string sourcePath, string destinationPath, bool overwrite)
}

#endif

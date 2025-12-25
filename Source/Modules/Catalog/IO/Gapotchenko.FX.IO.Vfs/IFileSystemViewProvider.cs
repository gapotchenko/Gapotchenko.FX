// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs;

/// <summary>
/// Provides <see cref="IFileSystemView"/> instance.
/// </summary>
/// <remarks>
/// This interface is useful in conjunction with <see cref="FileSystemView.For(IFileSystemViewProvider)"/> method.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IFileSystemViewProvider : IFileSystemViewProvisionIntent, IReadOnlyFileSystemViewProvider
{
    /// <summary>
    /// Gets the file system view.
    /// </summary>
    new IFileSystemView FileSystemView { get; }
}

/// <summary>
/// Provides <see cref="IReadOnlyFileSystemView"/> instance.
/// </summary>
/// <remarks>
/// This interface is useful in conjunction with <see cref="FileSystemView.For(IReadOnlyFileSystemViewProvider)"/> method.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IReadOnlyFileSystemViewProvider : IReadOnlyFileSystemViewProvisionIntent
{
    /// <summary>
    /// Gets the file system view.
    /// </summary>
    IReadOnlyFileSystemView FileSystemView { get; }
}

/// <summary>
/// Declares the intent of providing an <see cref="IFileSystemView"/> instance.
/// </summary>
/// <remarks>
/// <para>
/// A type implementing this interface should also implement <see cref="IFileSystemViewProvider"/> interface.
/// </para>
/// <para>
/// This interface is useful in conjunction with <see cref="FileSystemView.For(IFileSystemViewProvisionIntent)"/> method.
/// </para>
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IFileSystemViewProvisionIntent : IReadOnlyFileSystemViewProvisionIntent
{
}

/// <summary>
/// Declares the intent of providing an <see cref="IReadOnlyFileSystemView"/> instance.
/// </summary>
/// <remarks>
/// <para>
/// A type implementing this interface should also implement <see cref="IReadOnlyFileSystemViewProvider"/> interface.
/// </para>
/// <para>
/// This interface is useful in conjunction with <see cref="FileSystemView.For(IReadOnlyFileSystemViewProvisionIntent)"/> method.
/// </para>
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IReadOnlyFileSystemViewProvisionIntent
{
}

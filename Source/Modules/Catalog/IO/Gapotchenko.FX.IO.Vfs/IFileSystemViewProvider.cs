// Gapotchenko.FX
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
public interface IFileSystemViewProvider : ICanProvideFileSystemView, IReadOnlyFileSystemViewProvider
{
    /// <summary>
    /// Gets the file-system view.
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
public interface IReadOnlyFileSystemViewProvider : ICanProvideReadOnlyFileSystemView
{
    /// <summary>
    /// Gets the file-system view.
    /// </summary>
    IReadOnlyFileSystemView FileSystemView { get; }
}

/// <summary>
/// Declares the intent of providing an <see cref="IFileSystemView"/> instance.
/// </summary>
/// <remarks>
/// This interface is useful in conjunction with <see cref="FileSystemView.For(ICanProvideFileSystemView)"/> method.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ICanProvideFileSystemView : ICanProvideReadOnlyFileSystemView
{
}

/// <summary>
/// Declares the intent of providing an <see cref="IReadOnlyFileSystemView"/> instance.
/// </summary>
/// <remarks>
/// This interface is useful in conjunction with <see cref="FileSystemView.For(ICanProvideReadOnlyFileSystemView)"/> method.
/// </remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ICanProvideReadOnlyFileSystemView
{
}

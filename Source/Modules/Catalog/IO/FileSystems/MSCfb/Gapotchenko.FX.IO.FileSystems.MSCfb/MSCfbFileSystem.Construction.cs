// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.IO.Vfs;
using Gapotchenko.FX.IO.FileSystems.MSCfb.Impl;

namespace Gapotchenko.FX.IO.FileSystems.MSCfb;

partial class MSCfbFileSystem
{
    /// <summary>
    /// Initializes a new read-only instance of the <see cref="MSCfbFileSystem"/> class on the given stream.
    /// </summary>
    /// <param name="stream">The stream that contains the MS-CFB compound file to read.</param>
    public MSCfbFileSystem(Stream stream) :
        this(stream, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MSCfbFileSystem"/> class on the given stream
    /// with the <see cref="IFileSystemView.CanWrite"/> property set as specified.
    /// </summary>
    /// <param name="stream">The stream that contains the MS-CFB compound file.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property,
    /// which determines whether the file system supports writing.
    /// </param>
    public MSCfbFileSystem(Stream stream, bool writable) :
        this(stream, writable, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MSCfbFileSystem"/> class on the given stream
    /// with the <see cref="IFileSystemView.CanWrite"/> property set as specified,
    /// and optionally leaving the stream open.
    /// </summary>
    /// <param name="stream">The stream that contains the MS-CFB compound file.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property,
    /// which determines whether the file system supports writing.
    /// </param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <see cref="MSCfbFileSystem"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public MSCfbFileSystem(Stream stream, bool writable, bool leaveOpen) :
        this(stream, writable, leaveOpen, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MSCfbFileSystem"/> class on the given stream
    /// with the <see cref="IFileSystemView.CanWrite"/> property set as specified,
    /// optionally leaving the stream open,
    /// and using the specified options.
    /// </summary>
    /// <param name="stream">The stream that contains the MS-CFB compound file.</param>
    /// <param name="writable">
    /// The setting of the <see cref="IFileSystemView.CanWrite"/> property,
    /// which determines whether the file system supports writing.
    /// </param>
    /// <param name="leaveOpen">
    /// <see langword="true"/> to leave the stream open after the <see cref="MSCfbFileSystem"/> object is disposed;
    /// otherwise, <see langword="false"/>.
    /// </param>
    /// <param name="options">The MS-CFB file system options.</param>
    public MSCfbFileSystem(Stream stream, bool writable, bool leaveOpen, MSCfbFileSystemOptions? options) :
        this(stream, writable, leaveOpen, options, null)
    {
    }

    internal MSCfbFileSystem(Stream stream, bool writable, bool leaveOpen, MSCfbFileSystemOptions? options, VfsStorageContext? context,
        bool create = false)
    {
        ArgumentNullException.ThrowIfNull(stream);

        m_Writable = writable || create;

        m_Context = create
            ? CfbContext.Create(stream, leaveOpen)
            : CfbContext.Open(stream, leaveOpen, writable);

        if (options?.TrackLocation ?? true)
            Location = context?.Location;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            m_Context.Flush();
            m_Context.Dispose();
        }

        base.Dispose(disposing);
    }
}

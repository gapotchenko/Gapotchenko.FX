// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020
//
// Contributors:
//   - Oleksiy Gapotchenko (development)
//   - Kevin Gosse (fixes)

namespace Gapotchenko.FX.Diagnostics.Pal;

sealed class ProcessMemoryStream : Stream
{
    public ProcessMemoryStream(IProcessMemoryAccessor accessor, UniPtr baseAddress, long regionLength)
    {
        m_Accessor = accessor;
        m_BaseAddress = baseAddress;
        m_RegionLength = regionLength;

        m_PageSize = (uint)accessor.PageSize;
        m_FirstPageAddress = GetPageLowerBound(baseAddress);
    }

    readonly IProcessMemoryAccessor m_Accessor;
    readonly UniPtr m_BaseAddress;
    readonly long m_RegionLength;

    readonly uint m_PageSize;
    readonly UniPtr m_FirstPageAddress;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length
    {
        get
        {
            if (m_RegionLength == -1)
                throw new NotSupportedException();
            return m_RegionLength;
        }
    }

    long m_Position;

    public override long Position
    {
        get => m_Position;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            m_Position = value;
        }
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    UniPtr GetPageLowerBound(UniPtr address)
    {
        uint pageSize = m_PageSize;
        return new UniPtr(address.ToUInt64() / pageSize * pageSize);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (offset + count > buffer.Length)
            throw new ArgumentException();

        int totalCount = 0;
        do
        {
            var addr = m_BaseAddress + m_Position;

            var pageStart = GetPageLowerBound(addr);
            var pageEnd = pageStart + m_PageSize;

            int currentCount = count;

            ulong remainingPageSize = pageEnd.ToUInt64() - addr.ToUInt64();
            if ((ulong)currentCount > remainingPageSize)
                currentCount = (int)remainingPageSize;

            long regionLength = m_RegionLength;
            if (regionLength != -1)
            {
                long remainingRegionLength = regionLength - m_Position;
                if (currentCount > remainingRegionLength)
                    currentCount = (int)remainingRegionLength;
            }

            if (currentCount == 0)
            {
                // End of file (EOF).
                break;
            }

            // Accessing the first memory page should always work,
            // but if it doesn't then it denotes an exceptional situation.
            // Errors in accessing consequential pages are treated as an EOF condition.
            bool throwOnError = pageStart == m_FirstPageAddress;

            int r = m_Accessor.ReadMemory(addr, buffer, offset, currentCount, throwOnError);
            if (r <= 0)
            {
                if (throwOnError)
                {
                    // Just in case if a process memory adapter
                    // disregards the throw on error flag.
                    throw new IOException();
                }

                // Assume EOF.
                break;
            }

            count -= r;
            offset += r;
            totalCount += r;

            m_Position += r;
        }
        while (count > 0);

        return totalCount;
    }

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

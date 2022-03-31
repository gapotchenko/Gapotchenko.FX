using System;
using System.IO;

namespace Gapotchenko.FX.Diagnostics.Implementation
{
    sealed class ProcessMemoryStream : Stream
    {
        public ProcessMemoryStream(IProcessMemoryAdapter adapter, UniPtr baseAddress, long regionLength)
        {
            m_Adapter = adapter;
            m_BaseAddress = baseAddress;
            m_RegionLength = regionLength;

            m_PageSize = (uint)adapter.PageSize;
            m_FirstPageAddress = GetPageLowerBound(baseAddress);
        }

        IProcessMemoryAdapter m_Adapter;
        UniPtr m_BaseAddress;
        long m_RegionLength;

        uint m_PageSize;
        UniPtr m_FirstPageAddress;

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
            var pageSize = m_PageSize;
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

                var remainingPageSize = pageEnd.ToUInt64() - addr.ToUInt64();
                if ((ulong)currentCount > remainingPageSize)
                    currentCount = (int)remainingPageSize;

                var regionLength = m_RegionLength;
                if (regionLength != -1)
                {
                    long remainingRegionLength = regionLength - m_Position;
                    if (currentCount > remainingRegionLength)
                        currentCount = (int)remainingRegionLength;
                }

                if (currentCount == 0)
                {
                    // EOF
                    break;
                }

                bool throwOnError = pageStart == m_FirstPageAddress;

                int r = m_Adapter.ReadMemory(addr, buffer, offset, currentCount, throwOnError);
                if (r <= 0)
                {
                    if (throwOnError)
                    {
                        // In case if process memory adapter disregards a throw on error flag.
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
}

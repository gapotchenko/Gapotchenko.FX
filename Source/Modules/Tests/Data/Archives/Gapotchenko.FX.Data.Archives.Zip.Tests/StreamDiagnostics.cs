using Gapotchenko.FX.IO.Kits;

namespace Gapotchenko.FX.Data.Archives.Zip.Tests;

sealed class StreamDiagnostics(Stream baseStream) : StreamProxyKit(baseStream)
{
    public override void Close()
    {
        base.Close();
        IsClosed = true;
    }

    public bool IsClosed { get; private set; }
}

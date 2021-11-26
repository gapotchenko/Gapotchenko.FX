using System.IO;

namespace Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit
{
    abstract class ScanBuff
    {
        public const int EndOfFile = -1;
        public const int UnicodeReplacementChar = 0xFFFD;

        public bool IsFile => FileName != null;
        public string? FileName { get; set; }

        public abstract int Pos { get; set; }
        public abstract int Read();
        public virtual void Mark() { }

        public abstract string GetString(int begin, int limit);

        public static ScanBuff GetBuffer(TextReader reader)
        {
            return new TextReaderBuffer(reader);
        }
    }
}

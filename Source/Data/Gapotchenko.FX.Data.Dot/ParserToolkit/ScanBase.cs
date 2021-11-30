namespace Gapotchenko.FX.Data.Dot.ParserToolkit
{
    /// <summary>
    /// Abstract base class for GPLEX scanners.
    /// </summary>
    abstract class ScanBase : AbstractScanner<string, LexLocation>
    {
        public override LexLocation? yylloc { get; set; }
        protected virtual bool yywrap() => true;
    }
}

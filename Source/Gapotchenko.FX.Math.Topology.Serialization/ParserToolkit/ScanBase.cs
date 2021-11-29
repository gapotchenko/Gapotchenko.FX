namespace Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit
{
    /// <summary>
    /// Abstract base class for GPLEX scanners.
    /// </summary>
    abstract class ScanBase : AbstractScanner<DotValueType, LexLocation>
    {
        public override LexLocation? yylloc { get; set; }
        protected virtual bool yywrap() => true;
    }
}

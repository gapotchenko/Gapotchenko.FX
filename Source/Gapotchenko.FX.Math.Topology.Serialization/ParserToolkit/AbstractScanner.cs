namespace Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit
{
    abstract class AbstractScanner<TValue, TSpan>
        where TSpan : IMerge<TSpan>
    {
        /// <summary>
        /// Lexical value optionally set by the scanner. The value
        /// is of the %YYSTYPE type declared in the parser spec.
        /// </summary>
        /// <remarks>
        /// This is a traditional name for YACC-like functionality.<br />
        /// A field must be declared for this value of parametric type,
        /// since it may be instantiated by a value struct.  If it were 
        /// implemented as a property, machine generated code in derived
        /// types would not be able to select on the returned value.
        /// </remarks>
#pragma warning disable 649
        public TValue? yylval;                     // Lexical value: set by scanner
#pragma warning restore 649

        /// <summary>
        /// Current scanner location property. The value is of the
        /// type declared by %YYLTYPE in the parser specification.
        /// </summary>
        /// <remarks>This is a traditional name for YACC-like functionality.</remarks>
        public virtual TSpan? yylloc
        {
            get { return default; }                // Empty implementation allowing
            set { /* skip */ }                     // yylloc to be ignored entirely.
        }

        /// <summary>
        /// Main call point for LEX-like scanners.  Returns an int
        /// corresponding to the token recognized by the scanner.
        /// </summary>
        /// <returns>An int corresponding to the token</returns>
        /// <remarks>This is a traditional name for YACC-like functionality.</remarks>
        public abstract int yylex();

        /// <summary>
        /// Traditional error reporting provided by LEX-like scanners
        /// to their YACC-like clients.
        /// </summary>
        /// <param name="format">Message format string</param>
        /// <param name="args">Optional array of args</param>
        /// <remarks>This is a traditional name for YACC-like functionality.</remarks>
        public virtual void yyerror(string format, params object[] args) { }
    }
}

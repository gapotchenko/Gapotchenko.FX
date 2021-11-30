namespace Gapotchenko.FX.Data.Dot.ParserToolkit
{
    sealed class Rule
    {
        public int LeftHandSide; // symbol
        public int[] RightHandSide; // symbols

        /// <summary>
        /// Rule constructor.  This holds the ordinal of
        /// the left hand side symbol, and the list of
        /// right hand side symbols, in lexical order.
        /// </summary>
        /// <param name="left">The LHS non-terminal</param>
        /// <param name="right">The RHS symbols, in lexical order</param>
        public Rule(int left, int[] right)
        {
            LeftHandSide = left;
            RightHandSide = right;
        }
    }
}

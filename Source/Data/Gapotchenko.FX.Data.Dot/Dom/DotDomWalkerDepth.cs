namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Indicates how deep <see cref="DotDomWalker"/> should descend.
    /// </summary>
    public enum DotDomWalkerDepth
    {
        /// <summary>
        /// Descend only into nodes.
        /// </summary>
        Nodes,

        /// <summary>
        /// Descend into nodes and tokens.
        /// </summary>
        NodesAndTokens,

        /// <summary>
        /// Descend into nodes, tokens and trivia.
        /// </summary>
        NodesTokensAndTrivia
    }
}

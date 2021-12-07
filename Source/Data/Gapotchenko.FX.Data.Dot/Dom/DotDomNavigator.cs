namespace Gapotchenko.FX.Data.Dot.Dom
{
    static class DotDomNavigator
    {
        public static DotToken? TryGetFirstToken(DotNode current)
        {
            foreach (var child in current.ChildNodesAndTokens)
            {
                if (child is DotToken t)
                {
                    return t;
                }
                else if (child is DotNode n)
                {
                    var token = TryGetFirstToken(n);
                    if (token is not null)
                        return token;
                }
            }

            return default;
        }

        public static DotToken? TryGetLastToken(DotNode current)
        {
            foreach (var child in current.ChildNodesAndTokens.Reverse())
            {
                if (child is DotToken t)
                {
                    return t;
                }
                else if (child is DotNode n)
                {
                    var token = TryGetLastToken(n);
                    if (token is not null)
                        return token;
                }
            }

            return default;
        }
    }
}

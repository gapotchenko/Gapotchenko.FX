using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    static class DotDomNavigator
    {
        public static DotSignificantToken? TryGetFirstToken(DotNode current)
        {
            if (current is null)
                throw new ArgumentNullException(nameof(current));

            foreach (var child in current.ChildNodesAndTokens)
            {
                if (child is DotSignificantToken t)
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

        public static DotSignificantToken? TryGetLastToken(DotNode current)
        {
            if (current is null)
                throw new ArgumentNullException(nameof(current));

            foreach (var child in current.ChildNodesAndTokens.Reverse())
            {
                if (child is DotSignificantToken t)
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

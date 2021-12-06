using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    static class SyntaxNavigator
    {
        public static DotToken? GetFirstToken(DotNode current)
        {
            foreach (var child in current.ChildNodesAndTokens())
            {
                if (child.IsToken)
                {
                    return child.AsToken();
                }
                else if (child.IsNode)
                {
                    var token = GetFirstToken(child.AsNode()!);
                    if (token is not null)
                    {
                        return token;
                    }
                }
            }

            return default;
        }

        public static DotToken? GetLastToken(DotNode current)
        {
            foreach (var child in current.ChildNodesAndTokens().Reverse())
            {
                if (child.IsToken)
                {
                    return child.AsToken();
                }
                else if (child.IsNode)
                {
                    var token = GetLastToken(child.AsNode()!);
                    if (token is not null)
                    {
                        return token;
                    }
                }
            }

            return default;
        }
    }
}

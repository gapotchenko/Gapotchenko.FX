using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    static class ListDotSyntaxSlotProvider
    {
        public static ListDotSyntaxSlotProvider<TNode>? Create<TNode>(IList<TNode>? nodes)
            where TNode : DotNode
        {
            if (nodes is null)
                return null;
            return new(nodes);
        }
    }

    sealed class ListDotSyntaxSlotProvider<TNode> : IDotSyntaxSlotProvider
        where TNode : DotNode
    {
        readonly IList<TNode> _nodes;

        public ListDotSyntaxSlotProvider(IList<TNode> nodes)
        {
            _nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        }

        public int SlotCount => _nodes.Count;

        public IDotSyntaxSlotProvider GetSlot(int index) => _nodes[index];
    }
}

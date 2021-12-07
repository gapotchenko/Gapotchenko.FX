using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    struct SyntaxSlot
    {
        readonly DotElement? _element;
        readonly IReadOnlyList<DotNode>? _list;

        public SyntaxSlot(DotElement? element)
            : this(element, null)
        { }

        public SyntaxSlot(IReadOnlyList<DotNode>? list)
            : this(null, list)
        { }

        SyntaxSlot(
            DotElement? element,
            IReadOnlyList<DotNode>? list)
        {
            _element = element;
            _list = list;
        }

        public static implicit operator SyntaxSlot(DotElement? element) => new(element);

        public bool IsDefault => !IsElement && !IsList;

        [MemberNotNullWhen(true, nameof(Element))]
        public bool IsElement => _element != null;

        [MemberNotNullWhen(true, nameof(List))]
        public bool IsList => _list != null;

        public DotElement? Element => _element;

        public IReadOnlyList<DotNode>? List => _list;

        public int SlotCount =>
            _element is DotNode node ? node.SlotCount :
            _list != null ? _list is ISyntaxSlotProvider slotProvider ? slotProvider.SlotCount : _list.Count :
            0;

        public SyntaxSlot GetSlot(int index) =>
            _element is DotNode node ? node.GetSlot(index) :
            _list != null ? _list is ISyntaxSlotProvider slotProvider ? slotProvider.GetSlot(index) : _list[index] :
            throw new InvalidOperationException("A current slot has no child slots.");
    }
}

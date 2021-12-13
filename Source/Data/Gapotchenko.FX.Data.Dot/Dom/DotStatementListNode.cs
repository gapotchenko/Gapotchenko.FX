﻿using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document statement list.
    /// </summary>
    public sealed class DotStatementListNode : DotNode
    {
        /// <summary>
        /// Gets or sets <c>{</c> token.
        /// </summary>
        public DotPunctuationToken? OpenBraceToken { get; set; }

        /// <summary>
        /// Gets or sets a list of statements.
        /// </summary>
        public DotNodeList<DotStatementNode>? Statements { get; set; }

        /// <summary>
        /// Gets or sets <c>}</c> token.
        /// </summary>
        public DotPunctuationToken? CloseBraceToken { get; set; }

        internal override int SlotCount => 3;

        internal override DotSyntaxSlot GetSlot(int i) => i switch
        {
            0 => OpenBraceToken,
            1 => new DotSyntaxSlot(Statements),
            2 => CloseBraceToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotStatementListNode(this);
        }
    }
}
using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    partial class DotParser
    {
        static DotSyntaxTrivia CreateTrivia(
            DotToken tokenType,
            string value)
        {
            throw new NotImplementedException();
        }

        static DotSyntaxTrivia CreateWhitespaceTrivia(
            string value)
        {
            throw new NotImplementedException();
        }

        static DotVertexIdentifierSyntax CreateVertexIdentifierSyntax(
            DotSyntaxToken identifier,
            DotSyntaxToken colon1,
            DotSyntaxToken portIdentifier,
            DotSyntaxToken colon2,
            DotSyntaxToken compassPoint)
        {
            throw new NotImplementedException();
        }

        static DotAttributeSyntax CreateAttributeSyntax(
            DotSyntaxToken lhs,
            DotSyntaxToken equal,
            DotSyntaxToken rhs,
            DotSyntaxToken terminator)
        {
            throw new NotImplementedException();
        }

        static DotSyntaxList<DotAttributeListSyntax> CreateAttributeListSyntaxList(
            DotSyntaxToken openBraceToken,
            DotSyntaxList<DotAttributeSyntax> attributes,
            DotSyntaxToken closeBraceToken)
        {
            return new DotSyntaxList<DotAttributeListSyntax>
            {
                CreateAttributeListSyntax(openBraceToken, attributes, closeBraceToken)
            };
        }

        static DotAttributeListSyntax CreateAttributeListSyntax(
            DotSyntaxToken openBraceToken,
            DotSyntaxList<DotAttributeSyntax> attributes,
            DotSyntaxToken closeBraceToken)
        {
            return new DotAttributeListSyntax()
            {
                OpenBraceToken = openBraceToken,
                Attributes = attributes,
                CloseBraceToken = closeBraceToken,
            };
        }

        static AttachedDotAttributesSyntax CreateAttachedAttributesSyntax(
            DotSyntaxToken targetKeyword,
            DotSyntaxList<DotAttributeListSyntax> attributes)
        {
            throw new NotImplementedException();
        }

        static DotEdgeSyntax CreateEdgeSyntax(
            SeparatedDotSyntaxList<DotSyntaxNode> elements,
            DotSyntaxList<DotAttributeListSyntax> attributes)
        {
            throw new NotImplementedException();
        }

        static DotVertexSyntax CreateVertexSyntax(
            DotVertexIdentifierSyntax identifier,
            DotSyntaxList<DotAttributeListSyntax> attributes)
        {
            throw new NotImplementedException();
        }

        static DotAliasSyntax CreateAliasSyntax(
            DotSyntaxToken lhs,
            DotSyntaxToken equalToken,
            DotSyntaxToken rhs)
        {
            throw new NotImplementedException();
        }

        static DotSyntaxToken CreateToken(
            DotToken tokenType,
            string value)
        {
            return new DotSyntaxToken(tokenType, value);
        }

        static DotSyntaxToken CreateToken(
            DotValueType value)
        {
            return value.token;
        }

        static DotGraphSyntax CreateGraphSyntax(
            DotSyntaxToken strictKeyword,
            DotSyntaxToken graphKind,
            DotSyntaxToken identifier,
            DotStatementListSyntax statements)
        {
            throw new NotImplementedException();
        }

        static DotStatementListSyntax CreateStatementListSyntax(
            DotSyntaxToken openBrace,
            DotSyntaxList<DotStatementSyntax> statements,
            DotSyntaxToken closeBrace)
        {
            throw new NotImplementedException();
        }

        static void Prepend<T>(
            DotSyntaxList<T> syntaxList,
            T node)
        {
            throw new NotImplementedException();
        }

        static void Prepend<TNode>(
            SeparatedDotSyntaxList<TNode> syntaxList,
            TNode node)
            where TNode : DotSyntaxNode
        {
            throw new NotImplementedException();
        }
    }
}

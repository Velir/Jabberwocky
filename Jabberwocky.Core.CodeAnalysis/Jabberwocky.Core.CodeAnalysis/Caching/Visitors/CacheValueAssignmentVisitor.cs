using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jabberwocky.Core.CodeAnalysis.Caching.Visitors
{
	public class CacheValueAssignmentVisitor : CSharpSyntaxVisitor
	{
		private readonly ReturnValueWalker _nullValueWalker;

		private bool _hasPossibleNullValue;
		public bool HasPossibleNullValue => _hasPossibleNullValue || _nullValueWalker.PossibleNullValues.Any();

		public ICollection<SyntaxNode> PossibleNullValues => _nullValueWalker.PossibleNullValues; 

		public CacheValueAssignmentVisitor()
		{
			_nullValueWalker = new ReturnValueWalker();
		}

		public override void Visit(SyntaxNode node)
		{
			base.Visit(node);
		}

		public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
		{
			_nullValueWalker.Visit(node);
		}

		public override void VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			if (node.IsKind(SyntaxKind.NullLiteralExpression))
			{
				_hasPossibleNullValue = true;
			}
		}

		public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
		{
			_nullValueWalker.Visit(node);
		}

		public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
		{
			_nullValueWalker.Visit(node);
		}
	}
}

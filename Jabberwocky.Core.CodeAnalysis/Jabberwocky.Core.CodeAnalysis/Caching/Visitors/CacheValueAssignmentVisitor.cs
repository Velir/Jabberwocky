using System.Collections.Generic;
using System.Linq;
using Jabberwocky.Core.CodeAnalysis.Caching.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jabberwocky.Core.CodeAnalysis.Caching.Visitors
{
	public class CacheValueAssignmentVisitor : CSharpSyntaxVisitor
	{
		private readonly SyntaxNodeAnalysisContext _context;
		private readonly ReturnValueWalker _nullValueWalker;

		private bool _hasPossibleNullValue;
		public bool HasPossibleNullValue => _hasPossibleNullValue || _nullValueWalker.PossibleNullValues.Any();

		private readonly ICollection<SyntaxNode> _possibleNullValues = new List<SyntaxNode>(); 
		public ICollection<SyntaxNode> PossibleNullValues => _possibleNullValues.Concat(_nullValueWalker.PossibleNullValues).ToList(); 

		public CacheValueAssignmentVisitor(SyntaxNodeAnalysisContext context)
		{
			_context = context;
			_nullValueWalker = new ReturnValueWalker(context);
		}

		public override void DefaultVisit(SyntaxNode node)
		{
			// Fallback to walking the syntax tree
			_nullValueWalker.Visit(node);
			base.DefaultVisit(node);
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

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			// Is this a method invocation expression (anonymous delegate)?
			var methodDeclNode = CacheAnalysisUtil.GetMethodDeclarationNode(node, _context);
			if (methodDeclNode != null)
			{
				_nullValueWalker.Visit(methodDeclNode);
			}

			// For visiting a non-literal variable
			// perform data-flow analysis
			var possibleNullAssignments = CacheAnalysisUtil.GetNullAssignmentNodes(node, _context);
			foreach (var assignment in possibleNullAssignments)
			{
				_hasPossibleNullValue = true;
				_possibleNullValues.Add(assignment);
			}

			base.VisitIdentifierName(node);
		}

		public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
		{
			_nullValueWalker.MaxDepth = 2;
			_nullValueWalker.Visit(node);
		}

		public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
		{
			_nullValueWalker.MaxDepth = 2;
			_nullValueWalker.Visit(node);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jabberwocky.Core.CodeAnalysis.Caching.Visitors
{
	/// <summary>
	/// This class examines all return statements in a given Syntax Tree, and evaluates if any of them could potentially return null
	/// </summary>
	public class ReturnValueWalker : CSharpSyntaxWalker
	{
		private const uint MaxDepth = 1;

		private uint FunctionDepth { get; set; }

		public ICollection<SyntaxNode> PossibleNullValues = new List<SyntaxNode>();

		public ReturnValueWalker() : base(SyntaxWalkerDepth.Node)
		{
		}

		public override void VisitReturnStatement(ReturnStatementSyntax node)
		{
			if (FunctionDepth <= MaxDepth && node.Expression != null)
			{
				// If expression isn't null, then this is a function, not a method
				// Evaluate if this could be a null returning value
				if (node.Expression.IsKind(SyntaxKind.NullLiteralExpression))
				{
					PossibleNullValues.Add(node);

					// We can jump out of any further nested null-value resolution if we're sure this returns null
					return;
				} 
				/* TODO: Need to figure out how to handle null resolution for more complex return expressions
				 * Likely this will need to happen by maintaining a history of all local assignments, and rolling up nested null value
				 * assignments in reverse order, up to a maximum depth.
				*/ 
			}
			base.VisitReturnStatement(node);
		}

		public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
		{
			FunctionDepth++;
			base.VisitAnonymousMethodExpression(node);
			FunctionDepth--;
		}

		public override void VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			if (node.IsKind(SyntaxKind.NullLiteralExpression))
			{
				PossibleNullValues.Add(node);
				return;
			}

			base.VisitLiteralExpression(node);
		}

		public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
		{
			FunctionDepth++;
			base.VisitParenthesizedLambdaExpression(node);
			FunctionDepth--;
		}

		public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
		{
			FunctionDepth++;
			base.VisitSimpleLambdaExpression(node);
			FunctionDepth--;
		}

		public override void VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			FunctionDepth++;
			base.VisitInvocationExpression(node);
			FunctionDepth--;
		}
	}
}

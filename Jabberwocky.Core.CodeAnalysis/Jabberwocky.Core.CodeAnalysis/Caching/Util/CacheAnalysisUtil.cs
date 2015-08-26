using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jabberwocky.Core.CodeAnalysis.Caching.Util
{
	public static class CacheAnalysisUtil
	{
		public static IEnumerable<VariableDeclaratorSyntax> GetNullAssignmentNodes(IdentifierNameSyntax identifier, SyntaxNodeAnalysisContext context)
		{
			var symbol = context.SemanticModel.GetSymbolInfo(identifier).Symbol;
			var dfAnalysis = context.SemanticModel.AnalyzeDataFlow(identifier);
			var assignments =
				dfAnalysis.WrittenInside.Concat(dfAnalysis.WrittenOutside).Where(assignment => assignment.Equals(symbol));

			// Shouldn't there only ever be one assignment here?... maybe not with WrittenInside & WrittenOutside
			foreach (var assignment in assignments.Where(assig => assig.Locations.Length == 1))
			{
				var syntaxRef = assignment.DeclaringSyntaxReferences.FirstOrDefault();
				var assignmentNode = syntaxRef.GetSyntax(context.CancellationToken) as VariableDeclaratorSyntax;

				var valueExpression = assignmentNode?.Initializer.Value;
				if (valueExpression != null && valueExpression.IsKind(SyntaxKind.NullLiteralExpression))
				{
					yield return assignmentNode;
				}
			}
		}

		public static MethodDeclarationSyntax GetMethodDeclarationNode(ExpressionSyntax node, SyntaxNodeAnalysisContext context)
		{
			var methodSymbol = context.SemanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;
			if (methodSymbol == null) return null;

			var syntaxRef = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
			var methodDeclNode = syntaxRef.GetSyntax(context.CancellationToken) as MethodDeclarationSyntax; // should be BaseMethodDeclarationSyntax?

			return methodDeclNode;
		}
	}
}

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Jabberwocky.Core.CodeAnalysis.Caching.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jabberwocky.Core.CodeAnalysis.Caching
{
	public abstract class BaseCacheProviderNullValueAnalyzer : DiagnosticAnalyzer
	{
		public abstract string Id { get; }
		public abstract IImmutableSet<string> ValidMethodTargets { get; }
		public abstract IImmutableSet<string> ValidParameterNames { get; } 
		public abstract string AnalysisTypeTarget { get; }
		
		protected abstract DiagnosticDescriptor Rule { get; }

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			// Act at the method level
			context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.InvocationExpression);
		}

		protected virtual void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
		{
			var invocationNode = (InvocationExpressionSyntax)context.Node;

			// Is this a method invocation?
			IMethodSymbol methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbolInfo(invocationNode).Symbol;

			if (methodSymbol == null) return;

			// Is this method a member of the ISyncCacheProvider type?
			var cacheType = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalysisTypeTarget);
			var parentType = methodSymbol.ContainingType;

			if (!DoesTypeImplementAnalysisTarget(parentType, cacheType)) return;

			// We either already have the interface method, or we have to find it based on the more derived/concrete class being invoked
			var interfaceMethod = parentType.Equals(cacheType)
				? methodSymbol
				: cacheType.GetMembers(methodSymbol.Name)
					.FirstOrDefault(ifaceMember => methodSymbol.Equals(parentType.FindImplementationForInterfaceMember(ifaceMember)));

			if (interfaceMethod == null || !ValidMethodTargets.Contains(interfaceMethod.Name)) return;

			// Given the interface method, we only care about the 'value' or 'callback' parameters (or whatever they may be)
			var paramSymbol = methodSymbol.Parameters.FirstOrDefault(symbol => ValidParameterNames.Contains(symbol.Name));

			if (paramSymbol == null) return;

			// Parameter must either be constant, or Func<T> (includes method invocation, method body expression, or delegate/lambda)
			var paramPosition = paramSymbol.Ordinal;
			var argument = invocationNode.ArgumentList.Arguments[paramPosition];
			var argExpression = argument.Expression;

			var argumentVisitor = new CacheValueAssignmentVisitor(context);
			argExpression.Accept(argumentVisitor);

			if (argumentVisitor.HasPossibleNullValue)
			{
				var parentLocation = argExpression.GetLocation();

				var otherLocations = argumentVisitor.PossibleNullValues
					.Select(node => node.GetLocation())
					.Where(loc => LocationIsNotDuplicateOfParent(loc, parentLocation));

				CreateDiagnostic(context, parentLocation, otherLocations);
			}
		}

		protected virtual void CreateDiagnostic(SyntaxNodeAnalysisContext context, Location location, IEnumerable<Location> otherLocations)
		{
			var diagnostic = Diagnostic.Create(Rule, location, otherLocations);
			context.ReportDiagnostic(diagnostic);
		}

		private static bool LocationIsNotDuplicateOfParent(Location loc, Location parentLocation)
		{
			var locSpan = loc.GetLineSpan();
			var parentSpan = parentLocation.GetLineSpan();
			return !(locSpan.StartLinePosition >= parentSpan.StartLinePosition && locSpan.EndLinePosition <= parentSpan.EndLinePosition);
		}

		private static bool DoesTypeImplementAnalysisTarget(ITypeSymbol symbol, ITypeSymbol cacheType)
		{
			return symbol.Equals(cacheType) || symbol.AllInterfaces.Contains(cacheType);
		}
	}
}

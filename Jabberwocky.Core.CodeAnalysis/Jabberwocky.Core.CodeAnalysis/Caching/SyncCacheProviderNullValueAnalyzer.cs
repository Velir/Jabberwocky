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
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SyncCacheProviderNullValueAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "JabberwockyCoreCodeAnalysis.SyncCacheProvider.NullValue";

		private const string SyncCacheProviderTypeName = "Jabberwocky.Core.Caching.ISyncCacheProvider";
		private const string ValueParameterName = "value"; // is generic T where T : class
		private const string CallbackParameterName = "callback"; // is Func<T> where T : class
		private static readonly ISet<string> ValidMethodTargets = new HashSet<string> { "GetFromCache" }; 

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Usage";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			// Act at the method level
			context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.InvocationExpression);
		}

		private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
		{
			var invocationNode = (InvocationExpressionSyntax)context.Node;

			// Is this a method invocation?
			IMethodSymbol methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbolInfo(invocationNode).Symbol;

			if (methodSymbol == null) return;

			// Is this method a member of the ISyncCacheProvider type?
			var cacheType = context.SemanticModel.Compilation.GetTypeByMetadataName(SyncCacheProviderTypeName);
			var parentType = methodSymbol.ContainingType;

			if (!DoesTypeImplementSyncProvider(parentType, cacheType)) return;

			// We either already have the interface method, or we have to find it based on the more derived/concrete class being invoked
			var interfaceMethod = parentType.Equals(cacheType)
				? methodSymbol
				: cacheType.GetMembers(methodSymbol.Name)
					.FirstOrDefault(ifaceMember => methodSymbol.Equals(parentType.FindImplementationForInterfaceMember(ifaceMember)));

			if (interfaceMethod == null || !ValidMethodTargets.Contains(interfaceMethod.Name)) return;

			// Given the interface method, we only care about the 'value' or 'callback' parameters
			var paramSymbol =
				methodSymbol.Parameters.FirstOrDefault(
					symbol => symbol.Name.Equals(ValueParameterName) || symbol.Name.Equals(CallbackParameterName));

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

		private static bool LocationIsNotDuplicateOfParent(Location loc, Location parentLocation)
		{
			var locSpan = loc.GetLineSpan();
			var parentSpan = parentLocation.GetLineSpan();
			return !(locSpan.StartLinePosition >= parentSpan.StartLinePosition && locSpan.EndLinePosition <= parentSpan.EndLinePosition);
		}

		private static void CreateDiagnostic(SyntaxNodeAnalysisContext context, Location location, IEnumerable<Location> otherLocations)
		{
			var diagnostic = Diagnostic.Create(Rule, location, otherLocations);
			context.ReportDiagnostic(diagnostic);
		}

		private static bool DoesTypeImplementSyncProvider(ITypeSymbol symbol, ITypeSymbol cacheType)
		{
			return symbol.Equals(cacheType) || symbol.AllInterfaces.Contains(cacheType);
		}
	}
}

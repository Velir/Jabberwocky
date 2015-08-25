using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
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

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Usage";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
		private static SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			// Act at the method level
			context.RegisterSyntaxNodeAction(AnalyzeSymbol, SyntaxKind.InvocationExpression);
		}

		private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
		{
			var invocationNode = (InvocationExpressionSyntax)context.Node;

			// DIs this a method invocation?
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

			if (interfaceMethod == null) return;

			// Given the interface method, we only care about the 'value' or 'callback' parameters
			var paramSymbol =
				methodSymbol.Parameters.FirstOrDefault(
					symbol => symbol.Name.Equals(ValueParameterName) || symbol.Name.Equals(CallbackParameterName));

			if (paramSymbol == null) return;

			// Parameter must either be constant, or Func<T> (includes method invocation, method body expression, or delegate/lambda)
			// TODO: Need to parse the param expression... ie. constant/method body/delegate (should we use the visitor pattern?)
			if (paramSymbol.Name == ValueParameterName)
			{
				var paramPosition = paramSymbol.Ordinal;
				var argument = invocationNode.ArgumentList.Arguments[paramPosition];
				if (argument.Expression is LiteralExpressionSyntax)
				{
					var litExpression = argument.Expression as LiteralExpressionSyntax;
					if (litExpression.Kind() == SyntaxKind.NullLiteralExpression)
					{
						CreateDiagnostic(context, litExpression.GetLocation());
					}
				}
			}
			else if (paramSymbol.Name == CallbackParameterName)
			{
				
			}
        }

		private static void CreateDiagnostic(SyntaxNodeAnalysisContext context, Location location)
		{
			var diagnostic = Diagnostic.Create(Rule, location);
			context.ReportDiagnostic(diagnostic);
		}

		private static bool ParamIsFuncType(IParameterSymbol paramSymbol)
		{
			// TODO: Not sure what the format should be yet for generic Func<>
			return paramSymbol.ToDisplayString(_symbolDisplayFormat) == "Func";
		}

		private static bool ParamIsObjectType(SyntaxNodeAnalysisContext context, IParameterSymbol paramSymbol)
		{
			return paramSymbol.Type.Equals(context.SemanticModel.Compilation.GetSpecialType(SpecialType.System_Object));
		}

		private static bool DoesTypeImplementSyncProvider(ITypeSymbol symbol, ITypeSymbol cacheType)
		{
			return symbol.Equals(cacheType) || symbol.AllInterfaces.Contains(cacheType);
		}
	}
}

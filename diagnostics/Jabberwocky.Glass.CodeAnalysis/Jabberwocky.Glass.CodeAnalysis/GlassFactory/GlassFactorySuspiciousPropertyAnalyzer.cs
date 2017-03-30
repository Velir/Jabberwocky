using System.Collections.Immutable;
using System.Linq;
using Jabberwocky.Glass.CodeAnalysis.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jabberwocky.Glass.CodeAnalysis.GlassFactory
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GlassFactorySuspiciousPropertyAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "JabberwockyGlassCodeAnalysisGlassAdapterFactorySuspiciousProperty";
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SuspiciousPropertyTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SuspiciousPropertyMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SuspiciousPropertyDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Usage";

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Property);
		}

		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var propertySymbol = (IPropertySymbol)context.Symbol;

			// need to discover if containing type is a glass interface factory type
			var parentType = propertySymbol.ContainingType;
			if (!GlassFactoryAnalyzerUtil.IsTypeGlassFactoryType(parentType)
				|| propertySymbol.IsAbstract
				|| !IsDefaultGetProperty(propertySymbol))
				return;

			// We're a valid analysis target
			foreach (var location in propertySymbol.Locations)
			{
				var diagnostic = Diagnostic.Create(Rule, location, propertySymbol.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}

		private static bool IsDefaultGetProperty(IPropertySymbol propertySymbol)
		{
			// we need to search through all backing fields of the parent class
			var parentSymbol = propertySymbol.ContainingType;
			var hasAutoBackingField = parentSymbol.GetMembers().OfType<IFieldSymbol>().Any(symbol => propertySymbol.Equals(symbol.AssociatedSymbol));
			var isImplementedMember = parentSymbol.AllInterfaces
				.SelectMany(iface => iface.GetMembers(propertySymbol.Name))
				.Any(ifaceMember => propertySymbol.Equals(parentSymbol.FindImplementationForInterfaceMember(ifaceMember)));

			var hasExplicitGetter = GetterHasExplicitBodyOrInitializer(propertySymbol);

			return hasAutoBackingField && isImplementedMember && !hasExplicitGetter;
		}

		private static bool GetterHasExplicitBodyOrInitializer(IPropertySymbol symbol)
		{
			var propertyDeclSyntax = (PropertyDeclarationSyntax) symbol.DeclaringSyntaxReferences.First().GetSyntax();
			var getter = propertyDeclSyntax.AccessorList?.Accessors.FirstOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
			
			var body = getter?.Body;
			return body != null || propertyDeclSyntax.Initializer != null;
		}
	}
}
using System.Collections.Immutable;
using System.Linq;
using Jabberwocky.Glass.CodeAnalysis.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static Jabberwocky.Glass.CodeAnalysis.Util.GlassFactoryAnalyzerUtil;

namespace Jabberwocky.Glass.CodeAnalysis.GlassFactory
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GlassFactoryBaseInterfaceTypesMatchAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "JabberwockyGlassCodeAnalysis.BaseInterface.TypesMatch";
		internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.BaseInterfaceTitle), Resources.ResourceManager, typeof(Resources));
		internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.BaseInterfaceMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.BaseInterfaceDescription), Resources.ResourceManager, typeof(Resources));
		internal const string Category = "Usage";

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
		}

		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

			// need to find out if Attribute is assigned to class
			var attribute = context.Symbol.GetAttributes().FirstOrDefault(IsGlassFactoryTypeAttribute);
			
			// Are we a valid analysis target?
			if (attribute == null || !GlassFactoryAnalyzerUtil.InheritsBaseInterfaceClass(namedTypeSymbol) 
				|| DoesAttributeTypeMatchBaseInterfaceGenericType(attribute, namedTypeSymbol, context.Compilation))
				return;

			// We're a valid analysis target
			foreach (var location in namedTypeSymbol.Locations)
			{
				var diagnostic = Diagnostic.Create(Rule, location, namedTypeSymbol.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}

		private static bool DoesAttributeTypeMatchBaseInterfaceGenericType(AttributeData attribute, INamedTypeSymbol namedTypeSymbol, Compilation compilation)
		{
			var typeParam = attribute.ConstructorArguments.FirstOrDefault().Value as INamedTypeSymbol;
			
			if (typeParam == null) return false;

			var genericGlassType = namedTypeSymbol.BaseType.TypeArguments.FirstOrDefault();
			var conversion = compilation.ClassifyConversion(typeParam, genericGlassType);
            return GetFullyQualifiedTypeName(typeParam) == GetFullyQualifiedTypeName(genericGlassType)
				|| conversion.IsExplicit || conversion.IsImplicit;
		}
	}
}
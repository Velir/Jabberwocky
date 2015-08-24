using System.Collections.Immutable;
using System.Linq;
using Jabberwocky.Glass.CodeAnalysis.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jabberwocky.Glass.CodeAnalysis.GlassFactory
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class GlassFactoryTypeIsAbstractAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "JabberwockyGlassCodeAnalysis.GlassInterfaceType.NotAbstract";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Usage";

		private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
		}

		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

			// need to find out if Attribute is assigned to class
			var attribute = context.Symbol.GetAttributes().FirstOrDefault(GlassFactoryAnalyzerUtil.IsGlassFactoryTypeAttribute);
			
			// Are we a valid analysis target?
			if (attribute == null || namedTypeSymbol.IsAbstract) return;

			// We're a valid analysis target
			foreach (var location in namedTypeSymbol.Locations)
			{
				var diagnostic = Diagnostic.Create(Rule, location, namedTypeSymbol.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}

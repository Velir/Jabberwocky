using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Jabberwocky.Core.CodeAnalysis.Caching
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AsyncCacheProviderNullValueAnalyzer : BaseCacheProviderNullValueAnalyzer
	{
		public const string DiagnosticId = "JabberwockyCoreCodeAnalysis.AsyncCacheProvider.NullValue";

		public override string Id => DiagnosticId;
		public override string AnalysisTypeTarget => "Jabberwocky.Core.Caching.IAsyncCacheProvider";
		public override IImmutableSet<string> ValidMethodTargets => ImmutableHashSet.Create("GetFromCacheAsync");
		public override IImmutableSet<string> ValidParameterNames => ImmutableHashSet.Create("callback");

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.SyncCacheNullValueAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private const string Category = "Usage";

		protected override DiagnosticDescriptor Rule => new DiagnosticDescriptor(Id, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
	}
}

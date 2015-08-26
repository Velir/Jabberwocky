using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Jabberwocky.Core.CodeAnalysis;
using Jabberwocky.Core.CodeAnalysis.Caching;

namespace Jabberwocky.Core.CodeAnalysis.Test
{
	[TestClass]
	public class SyncCacheProviderTests : CodeFixVerifier
	{

		#region Test Compilation Source

		private const string SyncCache_NullCacheValues_Source = @"

	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using Jabberwocky.Core.Caching;

	public class MainClass {
		private readonly ISyncCacheProvider _syncCache;

		public MainClass(ISyncCacheProvider syncCache) {
			_syncCache = syncCache;
		}

		public void DoStuff() {
			_syncCache.GetFromCache<object>(""key"", () => null);
			_syncCache.AddToCache<object>(""key"", null);
		}
	}

";

		#endregion

		//No diagnostics expected to show up
		[TestMethod]
		public void TestMethod1()
		{
			// 19, 43
			// 20, 41

			var expected1 = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 43)
						}
			};

			var expected2 = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 20, 41)
						}
			};

			VerifyCSharpDiagnostic(SyncCache_NullCacheValues_Source, expected1, expected2);
		}

		//Diagnostic and CodeFix both triggered and checked for
		//[TestMethod]
		public void TestMethod2()
		{
			var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
        }
    }";
			var expected = new DiagnosticResult
			{
				Id = "JabberwockyCoreCodeAnalysis",
				Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 11, 15)
						}
			};

			VerifyCSharpDiagnostic(test, expected);

			var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";
			VerifyCSharpFix(test, fixtest);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new JabberwockyCoreCodeAnalysisCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new SyncCacheProviderNullValueAnalyzer();
		}
	}
}
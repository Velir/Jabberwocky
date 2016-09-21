using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
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
			_syncCache.GetFromCache<object>(""key"", () => null);  // invalid
			_syncCache.AddToCache<object>(""key"", null); // valid
		}
	}

";

		private const string SyncCache_LambdaWithNullReturnValue_Source = @"

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
			_syncCache.GetFromCache<string>(""key"", () => (string)null);
		}
	}

";

		private const string SyncCache_LambdaWithNullLiftedVariable_Source = @"

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
			string retVal = null;
			_syncCache.GetFromCache<string>(""key"", () => retVal);  // invalid
			_syncCache.AddToCache<string>(""key"", retVal); // valid
		}
	}

";

		private const string SyncCache_LambdaMethodInvocationWithPossibleNullReturnValue_Source = @"

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
			_syncCache.GetFromCache<string>(""key"", () => GetCacheValue());
		}

		private static string GetCacheValue() {
			int i = 0;
			if (i % 5 == 0) return null;

			return ""hello world"";
		}
	}

";

		private const string SyncCache_MethodExpressionInvocationWithPossibleNullReturnValue_Source = @"

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
			_syncCache.GetFromCache<string>(""key"", GetCacheValue);
		}

		private static string GetCacheValue() {
			int i = 0;
			if (i % 5 == 0) return null;

			return ""hello world"";
		}
	}

";

		#endregion

		[TestMethod]
		public void SyncCacheProvider_BasicNullValues_Analysis()
		{
			// 19, 43
			// 20, 41

			var expected = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 43)
						}
			};

			VerifyCSharpDiagnostic(SyncCache_NullCacheValues_Source, expected);
		}

		[TestMethod]
		public void SyncCacheProvider_CastExpressionNullValue_Analysis()
		{
			// 19, 43
			var expected = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 43)
						}
			};

			VerifyCSharpDiagnostic(SyncCache_LambdaWithNullReturnValue_Source, expected);
		}

		[TestMethod]
		public void SyncCacheProvider_LiftedNullValue_Analysis()
		{
			// 20, 43
			var expected = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 20, 43),
                            new DiagnosticResultLocation("Test0.cs", 19, 11)
						}
			};

			VerifyCSharpDiagnostic(SyncCache_LambdaWithNullLiftedVariable_Source, expected);
		}

		[TestMethod]
		public void SyncCacheProvider_MethodInvocationWithPossibleNullValue_Analysis()
		{
			// 20, 43
			// 21, 41
			var expected = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 43),
							new DiagnosticResultLocation("Test0.cs", 24, 20) // return null
						}
			};

			VerifyCSharpDiagnostic(SyncCache_LambdaMethodInvocationWithPossibleNullReturnValue_Source, expected);
		}

		[TestMethod]
		public void SyncCacheProvider_MethodExpressionInvocationWithPossibleNullValue_Analysis()
		{
			// 21, 43
			var expected = new DiagnosticResult
			{
				Id = SyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 43),
                            new DiagnosticResultLocation("Test0.cs", 24, 20) // return null
						}
			};
		
			VerifyCSharpDiagnostic(SyncCache_MethodExpressionInvocationWithPossibleNullReturnValue_Source, expected);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new SyncCacheProviderNullValueAnalyzer();
		}
	}
}
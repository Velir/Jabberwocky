using Jabberwocky.Core.CodeAnalysis.Caching;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Jabberwocky.Core.CodeAnalysis.Test
{
	[TestClass]
	public class AsyncCacheProviderTests : CodeFixVerifier
	{

		#region Test Compilation Source

		private const string AsyncCache_NullCacheValues_Source = @"

	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using Jabberwocky.Core.Caching;

	public class MainClass {
		private readonly IAsyncCacheProvider _asyncCache;

		public MainClass(IAsyncCacheProvider asyncCache) {
			_asyncCache = asyncCache;
		}

		public void DoStuff() {
			_asyncCache.GetFromCacheAsync<object>(""key"", () => null);  // invalid
			_asyncCache.AddToCacheAsync<object>(""key"", null); // valid
		}
	}

";

		private const string AsyncCache_LambdaWithNullReturnValue_Source = @"

	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using Jabberwocky.Core.Caching;

	public class MainClass {
		private readonly IAsyncCacheProvider _asyncCache;

		public MainClass(IAsyncCacheProvider asyncCache) {
			_asyncCache = asyncCache;
		}

		public void DoStuff() {
			_asyncCache.GetFromCacheAsync<string>(""key"", () => (string)null);
		}
	}

";

		private const string AsyncCache_LambdaWithNullLiftedVariable_Source = @"

	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using Jabberwocky.Core.Caching;

	public class MainClass {
		private readonly IAsyncCacheProvider _asyncCache;

		public MainClass(IAsyncCacheProvider asyncCache) {
			_asyncCache = asyncCache;
		}

		public void DoStuff() {
			string retVal = null;
			_asyncCache.GetFromCacheAsync<string>(""key"", () => retVal);  // invalid
			_asyncCache.AddToCacheAsync<string>(""key"", retVal); // valid
		}
	}

";

		private const string AsyncCache_LambdaMethodInvocationWithPossibleNullReturnValue_Source = @"

	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using Jabberwocky.Core.Caching;

	public class MainClass {
		private readonly IAsyncCacheProvider _asyncCache;

		public MainClass(IAsyncCacheProvider asyncCache) {
			_asyncCache = asyncCache;
		}

		public void DoStuff() {
			_asyncCache.GetFromCacheAsync<string>(""key"", () => GetCacheValue());
		}

		private static string GetCacheValue() {
			int i = 0;
			if (i % 5 == 0) return null;

			return ""hello world"";
		}
	}

";

		private const string AsyncCache_MethodExpressionInvocationWithPossibleNullReturnValue_Source = @"

	using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using Jabberwocky.Core.Caching;

	public class MainClass {
		private readonly IAsyncCacheProvider _asyncCache;

		public MainClass(IAsyncCacheProvider asyncCache) {
			_asyncCache = asyncCache;
		}

		public void DoStuff() {
			_asyncCache.GetFromCacheAsync<string>(""key"", GetCacheValue);
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
		public void AsyncCacheProvider_BasicNullValues_Analysis()
		{
			// 19, 49

			var expected = new DiagnosticResult
			{
				Id = AsyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 49)
						}
			};

			VerifyCSharpDiagnostic(AsyncCache_NullCacheValues_Source, expected);
		}

		[TestMethod]
		public void AsyncCacheProvider_CastExpressionNullValue_Analysis()
		{
			// 19, 49
			var expected = new DiagnosticResult
			{
				Id = AsyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 49)
						}
			};

			VerifyCSharpDiagnostic(AsyncCache_LambdaWithNullReturnValue_Source, expected);
		}

		[TestMethod]
		public void AsyncCacheProvider_LiftedNullValue_Analysis()
		{
			// 20, 49
			var expected = new DiagnosticResult
			{
				Id = AsyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 20, 49),
							new DiagnosticResultLocation("Test0.cs", 19, 11)
						}
			};

			VerifyCSharpDiagnostic(AsyncCache_LambdaWithNullLiftedVariable_Source, expected);
		}

		[TestMethod]
		public void AsyncCacheProvider_MethodInvocationWithPossibleNullValue_Analysis()
		{
			// 20, 49
			// 21, 42
			var expected = new DiagnosticResult
			{
				Id = AsyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 49),
							new DiagnosticResultLocation("Test0.cs", 24, 20) // return null
						}
			};

			VerifyCSharpDiagnostic(AsyncCache_LambdaMethodInvocationWithPossibleNullReturnValue_Source, expected);
		}

		[TestMethod]
		public void AsyncCacheProvider_MethodExpressionInvocationWithPossibleNullValue_Analysis()
		{
			// 21, 49
			var expected = new DiagnosticResult
			{
				Id = AsyncCacheProviderNullValueAnalyzer.DiagnosticId,
				Message = "The cached value cannot be null",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 19, 49),
							new DiagnosticResultLocation("Test0.cs", 24, 20) // return null
						}
			};

			VerifyCSharpDiagnostic(AsyncCache_MethodExpressionInvocationWithPossibleNullReturnValue_Source, expected);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new AsyncCacheProviderNullValueAnalyzer();
		}
	}
}

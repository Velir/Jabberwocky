using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace Jabberwocky.Glass.CodeAnalysis.Test
{
	[TestClass]
	public class UnitTest : CodeFixVerifier
	{
		private const string GlassFactoryType_NotAbstract_TestBadSource = @"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

[GlassFactoryType(typeof (IBaseType))]
public /*abstract*/ class IBaseTypeModel : ITestInterface
{
	
}

[GlassFactoryInterface]
public interface ITestInterface {
}

public interface IBaseType : IGlassBase
{
}

";

		private const string GlassFactoryType_NotAbstract_TestFixedSource = @"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

[GlassFactoryType(typeof (IBaseType))]
public /*abstract*/ abstract class IBaseTypeModel : ITestInterface
{
	
}

[GlassFactoryInterface]
public interface ITestInterface {
}

public interface IBaseType : IGlassBase
{
}

";

		[TestMethod]
		public void GlassFactoryType_ImplementingClass_IsAbstract_Analysis()
		{
			var expected = new DiagnosticResult
			{
				Id = GlassFactoryTypeIsAbstractAnalyzer.DiagnosticId,
				Message = String.Format("Type name '{0}' is not abstract", "IBaseTypeModel"),
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 17, 27)
						}
			};

			VerifyCSharpDiagnostic(GlassFactoryType_NotAbstract_TestBadSource, expected);


		}

		[TestMethod]
		public void GlassFactoryType_ImplementingClass_IsAbstract_Fix()
		{
			VerifyCSharpFix(GlassFactoryType_NotAbstract_TestBadSource, GlassFactoryType_NotAbstract_TestFixedSource);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new JabberwockyGlassCodeAnalysisCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new GlassFactoryTypeIsAbstractAnalyzer();
		}
	}
}
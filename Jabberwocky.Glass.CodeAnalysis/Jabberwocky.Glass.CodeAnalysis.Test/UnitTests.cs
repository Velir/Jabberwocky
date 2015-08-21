using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Jabberwocky.Glass.CodeAnalysis;

namespace Jabberwocky.Glass.CodeAnalysis.Test
{
	[TestClass]
	public class UnitTest : CodeFixVerifier
	{

		//No diagnostics expected to show up
		//[TestMethod]
		public void TestMethod1()
		{
			var test = @"";

			VerifyCSharpDiagnostic(test);
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
				Id = "JabberwockyGlassCodeAnalysis",
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

		[TestMethod]
		public void GlassFactoryType_ImplementingClass_IsAbstract()
		{
			var code = @"

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

			VerifyCSharpDiagnostic(code, expected);


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
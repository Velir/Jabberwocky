using Jabberwocky.Glass.CodeAnalysis.GlassFactory;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Jabberwocky.Glass.CodeAnalysis.Test
{
	[TestClass]
	public class GlassFactorySuspiciousPropertyTests : CodeFixVerifier
	{
		private enum CodeFix
		{
			MakeAbstract = 0,
			DefaultInitializer
		}

		#region Glass Factory BaseInterface Test Source

		private const string GlassFactory_HasSuspiciousProperty_Source = @"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interfaces;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

[GlassFactoryType(typeof (IBaseType))]
public abstract class IBaseTypeModel : BaseInterface<IBaseType>, ITestInterface
{
	public string Name { get; set; }
	public abstract object Hi { get; }
	public string World => ""hello"";
	public string CustomProperty => ""Ok Bye"";
}

[GlassFactoryInterface]
public interface ITestInterface {
	string Name { get; }
	object Hi { get; }
	string World { get; }
}

public interface IBaseType : IGlassBase
{
}

";

		private const string GlassFactory_HasSuspiciousProperty_MakeAbstract_Fix = @"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interfaces;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

[GlassFactoryType(typeof (IBaseType))]
public abstract class IBaseTypeModel : BaseInterface<IBaseType>, ITestInterface
{
	public abstract string Name { get; set; }
	public abstract object Hi { get; }
	public string World => ""hello"";
	public string CustomProperty => ""Ok Bye"";
}

[GlassFactoryInterface]
public interface ITestInterface {
	string Name { get; }
	object Hi { get; }
	string World { get; }
}

public interface IBaseType : IGlassBase
{
}

";

		private const string GlassFactory_HasSuspiciousProperty_InitializeDefault_Fix = @"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interfaces;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

[GlassFactoryType(typeof (IBaseType))]
public abstract class IBaseTypeModel : BaseInterface<IBaseType>, ITestInterface
{
	public string Name { get; set; } = default(string);
    public abstract object Hi { get; }
	public string World => ""hello"";
	public string CustomProperty => ""Ok Bye"";
}

[GlassFactoryInterface]
public interface ITestInterface {
	string Name { get; }
	object Hi { get; }
	string World { get; }
}

public interface IBaseType : IGlassBase
{
}

";

		private const string GlassFactory_NoSuspiciousProperties_Source = @"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interfaces;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

[GlassFactoryType(typeof (IBaseType))]
public abstract class IBaseTypeModel : BaseInterface<IBaseType>, ITestInterface
{
	public virtual string Name { get; set; } = ""A Constant Value"";
	public abstract object Hi { get; }
	public string World => ""hello"";
	public string Blah { get; } = ""blah"";
	public virtual string CustomProperty => ""Ok Bye"";
	public virtual string Bye { set; }
}

[GlassFactoryInterface]
public interface ITestInterface {
	string Name { get; }
	object Hi { get; }
	string World { get; }
	string Bye { set; }
}

public interface IBaseType : IGlassBase
{
}
";

		#endregion

		[TestMethod]
		public void GlassFactory_SuspiciousProperty_DefaultGetProperty_Analysis()
		{
			// 20, 16
			var expected = new DiagnosticResult
			{
				Id = GlassFactorySuspiciousPropertyAnalyzer.DiagnosticId,
				Message = "The property 'Name' should either be marked abstract, or specify an explicit implementation",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 20, 16)
						}
			};

			VerifyCSharpDiagnostic(GlassFactory_HasSuspiciousProperty_Source, expected);
		}

		[TestMethod]
		public void GlassFactory_SuspiciousProperty_DefaultGetProperty_MakeAbstract_CodeFix()
		{
			VerifyCSharpFix(GlassFactory_HasSuspiciousProperty_Source, GlassFactory_HasSuspiciousProperty_MakeAbstract_Fix, (int?)CodeFix.MakeAbstract);
		}

		[TestMethod]
		public void GlassFactory_SuspiciousProperty_DefaultGetProperty_InitializeDefaultValue_CodeFix()
		{
			VerifyCSharpFix(GlassFactory_HasSuspiciousProperty_Source, GlassFactory_HasSuspiciousProperty_InitializeDefault_Fix, (int?)CodeFix.DefaultInitializer);
		}

		[TestMethod]
		public void GlassFactory_NoSuspiciousProperties_Analysis()
		{
			VerifyCSharpDiagnostic(GlassFactory_NoSuspiciousProperties_Source);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new GlassFactorySuspiciousPropertyAnalyzer();
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new GlassFactorySuspiciousPropertyCodeFixProvider();
		}
	}
}

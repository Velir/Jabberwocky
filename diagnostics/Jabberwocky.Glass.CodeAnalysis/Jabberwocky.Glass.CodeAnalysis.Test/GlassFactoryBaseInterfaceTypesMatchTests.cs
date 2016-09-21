using Jabberwocky.Glass.CodeAnalysis.GlassFactory;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Jabberwocky.Glass.CodeAnalysis.Test
{
	[TestClass]
	public class GlassFactoryBaseInterfaceTypesMatchTests : CodeFixVerifier
	{
		#region Glass Factory BaseInterface Test Source

		private const string GlassFactory_BaseInterface_InvalidGenericParamter_Source = @"

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
public abstract class IBaseTypeModel : BaseInterface<string>, ITestInterface
{
	
}

[GlassFactoryInterface]
public interface ITestInterface {
}

public interface IBaseType : IGlassBase
{
}

";

		private const string GlassFactory_BaseInterface_AssignmentCompatibleGenericParameter_Source = @"

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
public abstract class IBaseTypeModel : BaseInterface<IGlassBase>, ITestInterface
{
	
}

[GlassFactoryInterface]
public interface ITestInterface {
}

public interface IBaseType : IGlassBase
{
}

";

		private const string GlassFactory_BaseInterface_ExactMatchGenericParameter_Source = @"

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
	
}

[GlassFactoryInterface]
public interface ITestInterface {
}

public interface IBaseType : IGlassBase
{
}

";

		#endregion

		[TestMethod]
		public void GlassFactory_BaseInterface_TypesMatch_InvalidGenericParameter_Analysis()
		{
			// 18, 23
			var expected = new DiagnosticResult
			{
				Id = GlassFactoryBaseInterfaceTypesMatchAnalyzer.DiagnosticId,
				Message = "Glass Interface Type and Base Interface generic type are incompatible",
				Severity = DiagnosticSeverity.Warning,
				Locations =
					new[] {
							new DiagnosticResultLocation("Test0.cs", 18, 23)
						}
			};

			VerifyCSharpDiagnostic(GlassFactory_BaseInterface_InvalidGenericParamter_Source, expected);
		}

		[TestMethod]
		public void GlassFactory_BaseInterface_TypesMatch_AssignmentCompatibleGenericParameter_Analysis()
		{
			VerifyCSharpDiagnostic(GlassFactory_BaseInterface_AssignmentCompatibleGenericParameter_Source);
		}

		[TestMethod]
		public void GlassFactory_BaseInterface_TypesMatch_ExactMatchGenericParameter_Analysis()
		{
			VerifyCSharpDiagnostic(GlassFactory_BaseInterface_ExactMatchGenericParameter_Source);
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new GlassFactoryBaseInterfaceTypesMatchAnalyzer();
		}
	}
}

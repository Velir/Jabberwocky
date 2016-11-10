using System.Collections.Generic;
using System.Linq;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Jabberwocky.Core.Testing;
using NUnit.Framework;

namespace Jabberwocky.Autofac.Tests.Modules.Aspected.Strategies
{
	[TestFixture]
	public class ClassProxyStrategyTests
	{
		[Test]
		public void Validate_PrivateFieldExistence_Invariants()
		{
			var activator = new ReflectionActivator(typeof(string), 
				new DefaultConstructorFinder(),
				new MostParametersConstructorSelector(), 
				Enumerable.Empty<Parameter>(),
				Enumerable.Empty<Parameter>());

			var dynActivator = DynamicWrapper.For(activator);

			IEnumerable<Parameter> parameters = null;
			IEnumerable<Parameter> properties = null;

			Assert.DoesNotThrow(() => parameters = dynActivator._defaultParameters);
			Assert.DoesNotThrow(() => properties = dynActivator._configuredProperties);

			Assert.NotNull(parameters);
			Assert.NotNull(properties);
		}
	}
}

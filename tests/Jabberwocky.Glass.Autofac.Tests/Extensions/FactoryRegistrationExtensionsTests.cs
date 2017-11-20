using Autofac;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace Jabberwocky.Glass.Autofac.Tests.Extensions
{
	[TestFixture]
	public class FactoryRegistrationExtensionsTests
	{
		private ContainerBuilder _builder;

		[SetUp]
		public void TestSetup()
		{
			_builder = new ContainerBuilder();
		}

		[Test]
		public void RegisterGlassFactory_NoAssemblies_ReturnsFactory()
		{
			_builder.RegisterGlassFactory();
			var container = _builder.Build();
			container.RegisterContainer();

			var factory = container.Resolve<IGlassInterfaceFactory>();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void RegisterGlassFactory_OptionsWithNoAssemblies_ReturnsFactory()
		{
			_builder.RegisterGlassFactory(new ConfigurationOptions());
			var container = _builder.Build();
			container.RegisterContainer();

			var factory = container.Resolve<IGlassInterfaceFactory>();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void RegisterGlassFactory_WithDebuggingAndNoAssemblies_ReturnsFactory()
		{
			_builder.RegisterGlassFactory(new ConfigurationOptions(true));
			var container = _builder.Build();
			container.RegisterContainer();

			var factory = container.Resolve<IGlassInterfaceFactory>();
			Assert.IsNotNull(factory);
		}

	}
}

using Autofac;
using Glass.Mapper.Sc;
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

			_builder.Register(c => Substitute.For<ISitecoreService>()).As<ISitecoreService>();
		}

		[Test]
		public void RegisterGlassFactory_NoAssemblies_ReturnsFactory()
		{
			_builder.RegisterGlassFactory();
			var container = _builder.Build();
			container.RegisterContainer();

			var factory = container.Resolve<IGlassAdapterFactory>();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void RegisterGlassFactory_OptionsWithNoAssemblies_ReturnsFactory()
		{
			_builder.RegisterGlassFactory(new ConfigurationOptions());
			var container = _builder.Build();
			container.RegisterContainer();

			var factory = container.Resolve<IGlassAdapterFactory>();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void RegisterGlassFactory_WithDebuggingAndNoAssemblies_ReturnsFactory()
		{
			_builder.RegisterGlassFactory(new ConfigurationOptions(true));
			var container = _builder.Build();
			container.RegisterContainer();

			var factory = container.Resolve<IGlassAdapterFactory>();
			Assert.IsNotNull(factory);
		}

	}
}

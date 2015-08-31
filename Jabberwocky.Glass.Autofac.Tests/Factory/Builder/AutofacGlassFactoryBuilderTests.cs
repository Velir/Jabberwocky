using System;
using System.Reflection;
using Autofac;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Factory.Builder;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Caching.Providers;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using NSubstitute;
using NUnit.Framework;

namespace Jabberwocky.Glass.Autofac.Tests.Factory.Builder
{
	[TestFixture]
	public class AutofacGlassFactoryBuilderTests
	{
		private IImplementationFactory _mockImplFactory;
		private ISitecoreService _mockService;
		private IGlassTemplateCacheService _mockCache;
		private IGlassTypesLoader _mockTypeLoader;

		private IConfigurationOptions _mockOptions;
		private IContainer _mockContainer;

		// SUT
		private AutofacGlassFactoryBuilder _builder;

		[SetUp]
		public void TestSetup()
		{
			_mockOptions = Substitute.For<IConfigurationOptions>();
			_mockOptions.Assemblies.Returns(new[] { Assembly.GetAssembly(GetType()).FullName });
			_mockContainer = Substitute.For<IContainer>();

			_mockImplFactory = Substitute.For<IImplementationFactory>();
			_mockService = Substitute.For<ISitecoreService>();
			_mockCache = Substitute.For<IGlassTemplateCacheService>();
			_mockTypeLoader = Substitute.For<IGlassTypesLoader>();

			_builder = new AutofacGlassFactoryBuilder(_mockOptions, _mockContainer, () => _mockService, lookup => _mockCache, _mockTypeLoader, _mockImplFactory);
		}

		[Test]
		public void BuildFactory_ReturnsFactory()
		{
			var factory = _builder.BuildFactory();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void BuildFactory_EmptyAssemblies_ReturnsFactory()
		{
			_mockOptions.Assemblies.Returns(new string[0]);

			var factory = _builder.BuildFactory();
			Assert.IsNotNull(factory);
		}

		[Test]
		public void BuildFactory_BadAssembly_ReturnsFactory()
		{
			_mockOptions.Assemblies.Returns(new[] { "bad, I don't exist" });

			var factory = _builder.BuildFactory();
			Assert.IsNotNull(factory);
		}
	}
}

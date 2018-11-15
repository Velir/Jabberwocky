using System;
using System.Reflection;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Builder;
using Jabberwocky.Glass.Factory.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace Jabberwocky.Glass.Tests.Factory.Builder
{
	[TestFixture]
	public class DefaultGlassFactoryBuilderTests
	{
		private ISitecoreService _mockService;

		private IConfigurationOptions _mockOptions;
		private IServiceProvider _mockProvider;

		// SUT
		private DefaultGlassFactoryBuilder _builder;

		[SetUp]
		public void TestSetup()
		{
			_mockProvider = Substitute.For<IServiceProvider>();
			_mockOptions = Substitute.For<IConfigurationOptions>();
			_mockOptions.Assemblies.Returns(new[] {Assembly.GetAssembly(GetType()).FullName});
			
			_mockService = Substitute.For<ISitecoreService>();

			_builder = new DefaultGlassFactoryBuilder(_mockOptions, () => _mockService, _mockProvider);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public void Constructor_NullOptions_Throws()
		{
			new DefaultGlassFactoryBuilder(null, () => _mockService, _mockProvider);
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

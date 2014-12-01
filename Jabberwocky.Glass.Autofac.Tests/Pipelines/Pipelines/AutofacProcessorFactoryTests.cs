using Autofac;
using Jabberwocky.Core.Testing;
using Jabberwocky.Glass.Autofac.Pipelines.Factories;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Jabberwocky.Glass.Autofac.Tests.Pipelines.Processors;
using Jabberwocky.Glass.Autofac.Util;
using NSubstitute;
using NUnit.Framework;

namespace Jabberwocky.Glass.Autofac.Tests.Pipelines.Pipelines
{
	[TestFixture]
	public class AutofacProcessorFactoryTests
	{
		private AutofacProcessorFactory _factory;
		private static readonly string TestProcessorFQN = typeof (TestProcessor).AssemblyQualifiedName;

		[SetUp]
		public void TestSetup()
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<TestProcessor>().AsSelf();
			builder.RegisterType<object>().AsSelf(); // dependency of TestProcessor

			AutofacConfig.ServiceLocator = builder.Build();

			// SUT
			_factory = new AutofacProcessorFactory();
        }

		[TestFixtureTearDown]
		public void TestCleanup()
		{
			AutofacConfig.ServiceLocator = null;
		}

		[Test]
		public void GetObject_NullType_ReturnsNull()
		{
			var service = _factory.GetObject(null);

			Assert.IsNull(service);
		}

		[Test]
		public void GetObject_EmptyStringType_ReturnsNull()
		{
			var service = _factory.GetObject(string.Empty);

			Assert.IsNull(service);
		}

		[Test]
		public void GetObject_InvalidType_ReturnsNull()
		{
			var service = _factory.GetObject("I absolutely don't exist");

			Assert.IsNull(service);
		}

		[Test]
		public void GetObject_ReturnsType_DisposesLifetime()
		{
			var service = _factory.GetObject(TestProcessorFQN) as ProcessorBase<string>;
			dynamic dynService = DynamicWrapper.For(service);

			var isDisposed = false;
			ILifetimeScope scope = dynService._lifetimeScope;
			scope.CurrentScopeEnding += (sender, args) => isDisposed = true;

			// Before
			Assert.IsNotNull(service);
			Assert.IsFalse(isDisposed);

			service.Process(null);

			// After
			Assert.IsTrue(isDisposed);
		}

		[Test]
		public void GetObject_ThrowsResolvingObject_DisposesLifetime()
		{
			// Arrange... override default settings
			var mockScope = Substitute.For<ILifetimeScope>();
			var mockContainer = Substitute.For<IContainer>();

			mockContainer.BeginLifetimeScope().Returns(mockScope);

			AutofacConfig.ServiceLocator = mockContainer;

			// Act

			var service = _factory.GetObject(TestProcessorFQN);

			// Assert

			Assert.IsNull(service);
			mockScope.Received().Dispose();
		}
	}
}

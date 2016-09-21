using Autofac;
using Jabberwocky.Autofac.Extensions;
using NUnit.Framework;

namespace Jabberwocky.Autofac.Tests.Extensions
{
    [TestFixture]
    public class ResolutionExtensionsTests
    {
        private ContainerBuilder _builder;

        [SetUp]
        public void Setup()
        {
            _builder = new ContainerBuilder();
        }

        [Test]
        public void ResolveWithoutExceptions_MissingRegistration_ReturnsNull()
        {
            var retVal = _builder.Build().ResolveWithoutExceptions<string>();

            Assert.IsNull(retVal);
        }

        [Test]
        public void ResolveWithoutExceptions_ThrowsOnResolve_ReturnsNull()
        {
            _builder.Register(c =>
            {
                Assert.Fail();
                return "I Failed!";
            });

            Assert.IsNull(_builder.Build().ResolveWithoutExceptions<string>());
        }

        [Test]
        public void TryResolveWithoutExceptions_MissingRegistration_ReturnsNull()
        {
            string retVal;

            Assert.IsFalse(_builder.Build().TryResolveWithoutExceptions(out retVal));
            Assert.IsNull(retVal);
        }

        [Test]
        public void TryResolveWithoutExceptions_ThrowsOnResolve_ReturnsNull()
        {
            _builder.Register(c =>
            {
                Assert.Fail();
                return "I Failed!";
            });

            string retVal;

            Assert.IsFalse(_builder.Build().TryResolveWithoutExceptions(out retVal));
            Assert.IsNull(retVal);
        }
    }
}

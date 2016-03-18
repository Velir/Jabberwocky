using System;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Mvc.Services;
using Jabberwocky.Glass.Models;
using Microsoft.QualityTools.Testing.Fakes;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Autofac.Mvc.Tests.Services
{
    [TestFixture]
    public class RenderingContextServiceTests
    {

        private Rendering _rendering;
        private RenderingContext _renderingContext;
        private IDisposable _disposableRenderingContext;
        private ISitecoreContext _sitecoreContext;

        private IGlassBase _directDatasource;
        private IGlassBase _staticItemDatasource;
        private IGlassBase _nestedItemDatasource;
        private IGlassBase _contextItem;

        private RenderingContextService _renderingService;

        [SetUp]
        public void Setup()
        {
            _rendering = Substitute.For<Rendering>();
            _renderingContext = Substitute.ForPartsOf<RenderingContext>();
            _renderingContext.Rendering = _rendering;

            _disposableRenderingContext = ContextService.Get().Push(_renderingContext);

            _directDatasource = Substitute.For<IGlassBase>();
            _staticItemDatasource = Substitute.For<IGlassBase>();
            _nestedItemDatasource = Substitute.For<IGlassBase>();
            _contextItem = Substitute.For<IGlassBase>();

            _directDatasource._Id.Returns(Guid.NewGuid());
            _staticItemDatasource._Id.Returns(Guid.NewGuid());
            _nestedItemDatasource._Id.Returns(Guid.NewGuid());
            _contextItem._Id.Returns(Guid.NewGuid());

            var mockGlassHtml = Substitute.For<IGlassHtml>();
            _sitecoreContext = Substitute.For<ISitecoreContext>();

            // SUT
            _renderingService = new RenderingContextService(mockGlassHtml, _sitecoreContext);
        }

        [TearDown]
        public void Cleanup()
        {
            _disposableRenderingContext?.Dispose();
        }

        [Test]
        public void GetCurrentRenderingDatasource_NestingEnabled_WithDirectDatasource_WithDefaultNesting_ReturnsDatasource()
        {
            // Setup DIRECT datasource for rendering
            _rendering.DataSource.Returns(ci => _directDatasource._Id.ToString());
            _sitecoreContext.GetItem<IGlassBase>(_directDatasource._Id, inferType: true).Returns(_directDatasource);

            var datasource = _renderingService.GetCurrentRenderingDatasource<IGlassBase>();

            Assert.AreEqual(_directDatasource._Id, datasource._Id);
        }

        [Test]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasource_WithDefaultNesting_ReturnsStaticItemFirst()
        {
            var props = new RenderingProperties(_rendering);

            using (ShimsContext.Create())
            {
                var fakeItem = CreateFakeItem(_staticItemDatasource._Id);

                // Setup rendering params: no direct datasource, rendering.Item is set to static item
                _rendering.DataSource.Returns(ci => null);
                _rendering.Properties.Returns(ci => props);
                _rendering.Item.Returns(fakeItem);

                _sitecoreContext.GetItem<IGlassBase>(_staticItemDatasource._Id, inferType: true).Returns(_staticItemDatasource);

                var datasource = _renderingService.GetCurrentRenderingDatasource<IGlassBase>();

                Assert.AreEqual(_staticItemDatasource._Id, datasource._Id);
            }
        }

        [Test]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasource_WithAlwaysNesting_ReturnsStaticItemFirst()
        {
            var props = new RenderingProperties(_rendering);

            // Setup rendering params: no direct datasource, static item is set
            _rendering.DataSource.Returns(ci => null);
            _rendering.Properties.Returns(ci => props);
            props["ItemId"] = _staticItemDatasource._Id.ToString();

            _sitecoreContext.GetItem<IGlassBase>(_staticItemDatasource._Id, inferType: true).Returns(_staticItemDatasource);

            var datasource = _renderingService.GetCurrentRenderingDatasource<IGlassBase>(DatasourceNestingOptions.Always);

            Assert.AreEqual(_staticItemDatasource._Id, datasource._Id);
        }

        [Test]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasourceOrStaticItem_WithAlwaysNesting_ReturnsNestedItem()
        {
            var props = new RenderingProperties(_rendering);

            using (ShimsContext.Create())
            {
                var fakeItem = CreateFakeItem(_nestedItemDatasource._Id);

                // Setup rendering params: no direct datasource, static item is set
                _rendering.DataSource.Returns(ci => null);
                _rendering.Properties.Returns(ci => props);
                props["ItemId"] = null; // simulates no StaticItem
                _renderingContext.ContextItem.Returns(fakeItem); // sets the nested datasource

                _sitecoreContext.GetItem<IGlassBase>(_nestedItemDatasource._Id, inferType: true)
                    .Returns(_nestedItemDatasource);

                var datasource =
                    _renderingService.GetCurrentRenderingDatasource<IGlassBase>(DatasourceNestingOptions.Always);

                Assert.AreEqual(_nestedItemDatasource._Id, datasource._Id);
            }
        }

        [Test]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasourceOrStaticItem_WithNeverNesting_ReturnsContextItem()
        {
            var props = new RenderingProperties(_rendering);

            using (ShimsContext.Create())
            {
                var fakeItem = CreateFakeItem(_nestedItemDatasource._Id);

                // Setup rendering params: no direct datasource, static item is set
                _rendering.DataSource.Returns(ci => null);
                _rendering.Properties.Returns(ci => props);
                props["ItemId"] = null; // simulates no StaticItem
                _renderingContext.ContextItem.Returns(fakeItem); // A nested datasource item IS set...

                _sitecoreContext.GetCurrentItem<IGlassBase>(inferType: true).Returns(_contextItem);

                var datasource =
                    _renderingService.GetCurrentRenderingDatasource<IGlassBase>(DatasourceNestingOptions.Never);

                Assert.AreEqual(_contextItem._Id, datasource._Id);
            }
        }

        private Item CreateFakeItem(Guid id)
        {
            return new Sitecore.Data.Items.Fakes.ShimItem
            {
                IDGet = () => Substitute.For<ID>(id)
            };
        }
    }
}

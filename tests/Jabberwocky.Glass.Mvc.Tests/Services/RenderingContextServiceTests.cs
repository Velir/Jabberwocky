using System;
using AutoSitecore;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web.Mvc;
using Jabberwocky.Glass.Models;
using Jabberwocky.Glass.Mvc.Services;
using Jabberwocky.Glass.Mvc.Tests.Util;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Mvc.Tests.Services
{
    [TestFixture]
    public class RenderingContextServiceTests
    {

        private Rendering _rendering;
        private RenderingContext _renderingContext;
        private IDisposable _disposableRenderingContext;
        private IMvcContext _sitecoreContext;

        private IGlassBase _directDatasource;
        private IGlassBase _staticItemDatasource;
        private IGlassBase _nestedItemDatasource;
        private IGlassBase _contextItem;

        private RenderingContextService _renderingService;

        #region ID Constants

        private const string StaticItemDatasourceId = "{011DE547-24FC-47B3-80DB-BBA29962FE84}";
        private const string NestedItemDatasourceId = "{4A75054B-B7A5-4181-A695-D5F5B8F2FB32}";

        #endregion
        
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
            _staticItemDatasource._Id.Returns(new Guid(StaticItemDatasourceId));
            _nestedItemDatasource._Id.Returns(new Guid(NestedItemDatasourceId));
            _contextItem._Id.Returns(Guid.NewGuid());

            var mockGlassHtml = Substitute.For<IGlassHtml>();
            _sitecoreContext = Substitute.For<IMvcContext>();

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
            _sitecoreContext.SitecoreService.GetItem<IGlassBase>(_directDatasource._Id, x => x.InferType()).Returns(_directDatasource);

            var datasource = _renderingService.GetCurrentRenderingDatasource<IGlassBase>();

            Assert.AreEqual(_directDatasource._Id, datasource._Id);
        }
        
        [Test, AutoSitecore]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasource_WithDefaultNesting_ReturnsStaticItemFirst([ItemData(itemId: StaticItemDatasourceId)] Item fakeItem)
        {
            var props = new RenderingProperties(_rendering);

            // Setup rendering params: no direct datasource, rendering.Item is set to static item
            _rendering.DataSource.Returns(ci => null);
            _rendering.Properties.Returns(ci => props);
            _rendering.Item.Returns(fakeItem);

            _sitecoreContext.SitecoreService.GetItem<IGlassBase>(_staticItemDatasource._Id, x => x.InferType()).Returns(_staticItemDatasource);

            var datasource = _renderingService.GetCurrentRenderingDatasource<IGlassBase>();

            Assert.AreEqual(_staticItemDatasource._Id, datasource._Id);
        }

        [Test]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasource_WithAlwaysNesting_ReturnsStaticItemFirst()
        {
            var props = new RenderingProperties(_rendering);

            // Setup rendering params: no direct datasource, static item is set
            _rendering.DataSource.Returns(ci => null);
            _rendering.Properties.Returns(ci => props);
            props["ItemId"] = _staticItemDatasource._Id.ToString();

            _sitecoreContext.SitecoreService.GetItem<IGlassBase>(_staticItemDatasource._Id, x => x.InferType()).Returns(_staticItemDatasource);

            var datasource = _renderingService.GetCurrentRenderingDatasource<IGlassBase>(DatasourceNestingOptions.Always);

            Assert.AreEqual(_staticItemDatasource._Id, datasource._Id);
        }

        [Test, AutoSitecore]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasourceOrStaticItem_WithAlwaysNesting_ReturnsNestedItem([ItemData(itemId: NestedItemDatasourceId)] Item fakeItem)
        {
            var props = new RenderingProperties(_rendering);

            // Setup rendering params: no direct datasource, static item is set
            _rendering.DataSource.Returns(ci => null);
            _rendering.Properties.Returns(ci => props);
            props["ItemId"] = null; // simulates no StaticItem
            _renderingContext.ContextItem.Returns(fakeItem); // sets the nested datasource

            _sitecoreContext.SitecoreService.GetItem<IGlassBase>(_nestedItemDatasource._Id, x => x.InferType())
                .Returns(_nestedItemDatasource);

            var datasource =
                _renderingService.GetCurrentRenderingDatasource<IGlassBase>(DatasourceNestingOptions.Always);

            Assert.AreEqual(_nestedItemDatasource._Id, datasource._Id);
        }

        [Test, AutoSitecore]
        public void GetCurrentRenderingDatasource_NestingEnabled_NoDirectDatasourceOrStaticItem_WithNeverNesting_ReturnsContextItem([ItemData(itemId: NestedItemDatasourceId)] Item fakeItem)
        {
            var props = new RenderingProperties(_rendering);

            // Setup rendering params: no direct datasource, static item is set
            _rendering.DataSource.Returns(ci => null);
            _rendering.Properties.Returns(ci => props);
            props["ItemId"] = null; // simulates no StaticItem
            _renderingContext.ContextItem.Returns(fakeItem); // A nested datasource item IS set...

            _sitecoreContext.GetPageContextItem<IGlassBase>(x => x.InferType()).Returns(_contextItem);

            var datasource =
                _renderingService.GetCurrentRenderingDatasource<IGlassBase>(DatasourceNestingOptions.Never);

            Assert.AreEqual(_contextItem._Id, datasource._Id);
        }
    }
}

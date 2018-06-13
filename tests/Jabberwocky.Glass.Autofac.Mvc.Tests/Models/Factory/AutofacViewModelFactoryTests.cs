using Autofac;
using Autofac.Core;
using Jabberwocky.Glass.Autofac.Mvc.Models.Factory;
using Jabberwocky.Glass.Models;
using Jabberwocky.Glass.Mvc.Models;
using Jabberwocky.Glass.Mvc.Models.Attributes;
using Jabberwocky.Glass.Mvc.Services;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Autofac.Mvc.Tests.Models.Factory
{
    [TestFixture]
    public class AutofacViewModelFactoryTests
    {
        private AutofacViewModelFactory _sut;
        private IComponentContext _resolver;
        private IRenderingContextService _renderingContextService;

        [SetUp]
        public void Setup()
        {
            _resolver = Substitute.For<IComponentContext>();
            _renderingContextService = Substitute.For<IRenderingContextService>();

            IComponentRegistration retVal;
            _resolver.ComponentRegistry.TryGetRegistration(null, out retVal).ReturnsForAnyArgs(true);

            _sut = new AutofacViewModelFactory(_resolver, _renderingContextService);
        }

        [Test]
        public void Create_InjectableGlassViewModel_SetsInternalModel()
        {
            var mockRendering = Substitute.For<Rendering>();
            _renderingContextService.GetCurrentRendering().Returns(mockRendering);

            var viewModel = new InjectableViewModel();
            var glassModel = Substitute.For<IGlassBase>();
            _resolver.ResolveOptional(typeof(object), new Parameter[0]).ReturnsForAnyArgs(viewModel);
            _renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);

            var resolvedModel = _sut.Create<InjectableViewModel>();

            Assert.AreSame(viewModel, resolvedModel);
            Assert.AreSame(glassModel, viewModel.GlassModel);
        }

        [Test]
        public void Create_DirectRenderingViewModel_SetsInternalModelAndRenderingParamModel()
        {
            var mockRendering = Substitute.For<Rendering>();
            _renderingContextService.GetCurrentRendering().Returns(mockRendering);

            var viewModel = new DirectRenderingViewModel();
            var glassModel = Substitute.For<IGlassBase>();
            var renderingModel = Substitute.For<IRenderingTemplate>();
            _resolver.ResolveOptional(typeof(object), new Parameter[0]).ReturnsForAnyArgs(viewModel);
            _renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);
            _renderingContextService.GetCurrentRenderingParameters(typeof(IRenderingTemplate))
                .ReturnsForAnyArgs(renderingModel);

            var resolvedModel = _sut.Create<DirectRenderingViewModel>();

            Assert.AreSame(viewModel, resolvedModel);
            Assert.AreSame(glassModel, viewModel.GlassModel);
            Assert.AreSame(renderingModel, viewModel.RenderingParameters);
        }

        [Test]
        public void Create_ViewModelWithConstructorParams_InjectsParams()
        {
            var mockRendering = Substitute.For<Rendering>();
            _renderingContextService.GetCurrentRendering().Returns(mockRendering);

            var glassModel = Substitute.For<IGlassBase>();
            var renderingModel = Substitute.For<IRenderingTemplate>();
            _resolver.ResolveOptional(typeof(object), new Parameter[0])
                .ReturnsForAnyArgs(ci => new ConstructorViewModel(GetValue<IRenderingTemplate>(ci[1], 1), GetValue<IGlassBase>(ci[1], 0)));
            _renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);
            _renderingContextService.GetCurrentRenderingParameters(typeof(IRenderingTemplate))
                .ReturnsForAnyArgs(renderingModel);

            var resolvedModel = _sut.Create<ConstructorViewModel>();

            Assert.AreSame(glassModel, resolvedModel.GlassModel);
            Assert.AreSame(renderingModel, resolvedModel.RenderingParameters);

            resolvedModel.AssertThatCctorInstancesAreSameAsProperties();
        }

        [Test]
        public void Create_WithNestedDatasourceAttribute_Never_UsesAppropriateDatasource()
        {
            var mockRendering = Substitute.For<Rendering>();
            _renderingContextService.GetCurrentRendering().Returns(mockRendering);

            var viewModel = new NeverFallbackViewModel();
            var glassModel = Substitute.For<IGlassBase>();
            _resolver.ResolveOptional(typeof(object), new Parameter[0]).ReturnsForAnyArgs(viewModel);
            _renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Never).Returns(glassModel);

            var resolvedModel = _sut.Create<NeverFallbackViewModel>();

            Assert.AreSame(viewModel, resolvedModel);
            Assert.AreSame(glassModel, viewModel.GlassModel);
            _renderingContextService.Received().GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Never);
        }

        [Test]
        public void Create_WithNestedDatasourceAttribute_Always_UsesAppropriateDatasource()
        {
            var mockRendering = Substitute.For<Rendering>();
            _renderingContextService.GetCurrentRendering().Returns(mockRendering);

            var viewModel = new AlwaysFallbackViewModel();
            var glassModel = Substitute.For<IGlassBase>();
            _resolver.ResolveOptional(typeof(object), new Parameter[0]).ReturnsForAnyArgs(viewModel);
            _renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Always).Returns(glassModel);

            var resolvedModel = _sut.Create<AlwaysFallbackViewModel>();

            Assert.AreSame(viewModel, resolvedModel);
            Assert.AreSame(glassModel, viewModel.GlassModel);
            _renderingContextService.Received().GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Always);
        }

        private T GetValue<T>(object ci, int index)
        {
            var @params = (Parameter[])ci;

            return (T)((TypedParameter)@params[index]).Value;
        }

        #region ViewModel Class Declarations

        private class InjectableViewModel : GlassViewModel<IGlassBase>
        {
        }

        private class DirectRenderingViewModel : GlassViewModel<IGlassBase, IRenderingTemplate>
        {
        }

        private class ConstructorViewModel : GlassViewModel<IGlassBase, IRenderingTemplate>
        {
            private readonly IRenderingTemplate _renderingModel;
            private readonly IGlassBase _datasourceModel;

            public ConstructorViewModel(IRenderingTemplate renderingModel, IGlassBase datasourceModel)
            {
                Assert.IsNotNull(renderingModel);
                Assert.IsNotNull(datasourceModel);

                _renderingModel = renderingModel;
                _datasourceModel = datasourceModel;
            }

            public void AssertThatCctorInstancesAreSameAsProperties()
            {
                Assert.AreSame(_renderingModel, RenderingParameters);
                Assert.AreSame(_datasourceModel, GlassModel);
            }
        }

        [DisableNestedDatasource]
        private class NeverFallbackViewModel : GlassViewModel<IGlassBase>
        {
        }

        [AllowNestedDatasource]
        private class AlwaysFallbackViewModel : GlassViewModel<IGlassBase>
        {
        }

        #endregion

        #region Template Declarations

        public interface IRenderingTemplate : IGlassBase
        {
        }

        #endregion

    }
}

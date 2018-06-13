using System;
using Jabberwocky.Glass.Models;
using Jabberwocky.Glass.Mvc.Models;
using Jabberwocky.Glass.Mvc.Models.Attributes;
using Jabberwocky.Glass.Mvc.Models.Factory;
using Jabberwocky.Glass.Mvc.Services;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Mvc.Tests.Models.Factory
{
	[TestFixture]
	public class AutofacViewModelFactoryTests
	{
		private ViewModelFactory _sut;
		private IServiceProvider _resolver;
		private IRenderingContextService _renderingContextService;

		[SetUp]
		public void Setup()
		{
			_resolver = Substitute.For<IServiceProvider>();
			_renderingContextService = Substitute.For<IRenderingContextService>();

			_sut = new ViewModelFactory(_resolver, _renderingContextService);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_DirectDerivedType_ReturnsCorrectGenericType()
		{
			var modelTuple = ViewModelFactory.InternalGetGlassModelAndRenderingTypesFromGenericParam(typeof(DirectViewModel)).Value;
			var datasourceType = modelTuple.GlassModel;
			Assert.IsNotNull(datasourceType);
			Assert.AreEqual(typeof(IGlassBase), datasourceType);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_IndirectDerivedType_ReturnsCorrectGenericType()
		{
			var modelTuple = ViewModelFactory.InternalGetGlassModelAndRenderingTypesFromGenericParam(typeof(IndirectViewModel)).Value;
			var datasourceType = modelTuple.GlassModel;
			Assert.IsNotNull(datasourceType);
			Assert.AreEqual(typeof(IGlassBase), datasourceType);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_IndirectGenericType_ReturnsCorrectGenericType()
		{
			var modelTuple = ViewModelFactory.InternalGetGlassModelAndRenderingTypesFromGenericParam(typeof(IndirectGenericViewModel)).Value;
			var datasourceType = modelTuple.GlassModel;
			Assert.IsNotNull(datasourceType);
			Assert.AreEqual(typeof(IGlassBase), datasourceType);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_NoInheritViewModel_ReturnsNull()
		{
			var modelTuple = ViewModelFactory.InternalGetGlassModelAndRenderingTypesFromGenericParam(typeof(NoInheritViewModel));
			Assert.IsNull(modelTuple);
		}

		[Test]
		public void GetGlassModelAndRenderingTypesFromGenericParam_DirectRenderingType_ReturnsCorrectRenderingModel()
		{
			var modelTuple = ViewModelFactory.InternalGetGlassModelAndRenderingTypesFromGenericParam(typeof(DirectRenderingViewModel)).Value;
			var datasourceType = modelTuple.GlassModel;
			var renderingParamType = modelTuple.RenderingParamModel;
			Assert.AreEqual(typeof(IGlassBase), datasourceType);
			Assert.AreEqual(typeof(IRenderingTemplate), renderingParamType);
		}

		[Test]
		public void Create_InjectableGlassViewModel_SetsInternalModel()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var viewModel = new InjectableViewModel();
			var glassModel = Substitute.For<IGlassBase>();
			_resolver.GetService(typeof(object)).ReturnsForAnyArgs(viewModel);
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
			_resolver.GetService(typeof(object)).ReturnsForAnyArgs(viewModel);
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);
			_renderingContextService.GetCurrentRenderingParameters(typeof(IRenderingTemplate))
					.ReturnsForAnyArgs(renderingModel);

			var resolvedModel = _sut.Create<DirectRenderingViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.GlassModel);
			Assert.AreSame(renderingModel, viewModel.RenderingParameters);
		}

		[Test]
		public void Create_WithNestedDatasourceAttribute_Never_UsesAppropriateDatasource()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var viewModel = new NeverFallbackViewModel();
			var glassModel = Substitute.For<IGlassBase>();
			_resolver.GetService(typeof(object)).ReturnsForAnyArgs(viewModel);
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
			_resolver.GetService(typeof(object)).ReturnsForAnyArgs(viewModel);
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Always).Returns(glassModel);

			var resolvedModel = _sut.Create<AlwaysFallbackViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.GlassModel);
			_renderingContextService.Received().GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Always);
		}

		#region ViewModel Class Declarations

		private abstract class DirectViewModel : GlassViewModel<IGlassBase>
		{
		}

		private abstract class GenericViewModel<U, T> : GlassViewModel<T> where T : class, IGlassBase
		{
		}

		private class IndirectViewModel : DirectViewModel
		{
		}

		private class IndirectGenericViewModel : GenericViewModel<object, IGlassBase>
		{
		}

		private class InjectableViewModel : GlassViewModel<IGlassBase>
		{
		}

		private class NoInheritViewModel
		{

		}

		private class DirectRenderingViewModel : GlassViewModel<IGlassBase, IRenderingTemplate>
		{
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

using System;
using Jabberwocky.Glass.Models;
using Jabberwocky.Glass.Mvc.Models;
using Jabberwocky.Glass.Mvc.Models.Attributes;
using Jabberwocky.Glass.Mvc.Models.Factory;
using Jabberwocky.Glass.Mvc.Services;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Mvc.Tests.Factory
{
	[TestFixture]
	public class ViewModelFactoryTests
	{
		private ViewModelFactory _sut;
		private IServiceProvider _provider;
		private IRenderingContextService _renderingContextService;

		[SetUp]
		public void Setup()
		{
			_provider = Substitute.For<IServiceProvider>();
			_renderingContextService = Substitute.For<IRenderingContextService>();
			
			_sut = new ViewModelFactory(_provider, _renderingContextService);
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
			_provider.GetService(typeof(IGlassBase)).ReturnsForAnyArgs(viewModel);
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);

			var resolvedModel = _sut.Create<InjectableViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.Datasource);
		}

		[Test]
		public void Create_DirectRenderingViewModel_SetsInternalModelAndRenderingParamModel()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var viewModel = new DirectRenderingViewModel();
			var glassModel = Substitute.For<IGlassBase>();
			var renderingModel = Substitute.For<IRenderingTemplate>();
			_provider.GetService(typeof(IGlassBase)).ReturnsForAnyArgs(viewModel);
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);
			_renderingContextService.GetCurrentRenderingParameters(typeof(IRenderingTemplate))
					.ReturnsForAnyArgs(renderingModel);

			var resolvedModel = _sut.Create<DirectRenderingViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.Datasource);
			Assert.AreSame(renderingModel, viewModel.RenderingParameters);
		}
		/*
		[Test]
		public void Create_ViewModelWithConstructorParams_InjectsParams()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var glassModel = Substitute.For<IGlassBase>();
			var renderingModel = Substitute.For<IRenderingTemplate>();
			_provider.GetService(Arg.Any<Type>())
					.ReturnsForAnyArgs(ci => new ConstructorViewModel(GetValue<IRenderingTemplate>(ci, 1), GetValue<IGlassBase>(ci, 0)));
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase)).ReturnsForAnyArgs(glassModel);
			_renderingContextService.GetCurrentRenderingParameters(typeof(IRenderingTemplate))
					.ReturnsForAnyArgs(renderingModel);

			var resolvedModel = _sut.Create<ConstructorViewModel>();

			Assert.AreSame(glassModel, resolvedModel.Datasource);
			Assert.AreSame(renderingModel, resolvedModel.RenderingParameters);

			resolvedModel.AssertThatCctorInstancesAreSameAsProperties();
		}
		*/
		[Test]
		public void Create_WithNestedDatasourceAttribute_Never_UsesAppropriateDatasource()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var viewModel = new NeverFallbackViewModel();
			var glassModel = Substitute.For<IGlassBase>();
			_provider.GetService(Arg.Any<Type>()).ReturnsForAnyArgs(viewModel);
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Never).Returns(glassModel);

			var resolvedModel = _sut.Create<NeverFallbackViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.Datasource);
			_renderingContextService.Received().GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Never);
		}

		[Test]
		public void Create_WithNestedDatasourceAttribute_Always_UsesAppropriateDatasource()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var viewModel = new AlwaysFallbackViewModel();
			var glassModel = Substitute.For<IGlassBase>();
			_provider.GetService(Arg.Any<Type>()).ReturnsForAnyArgs(viewModel);
			_renderingContextService.GetCurrentRenderingDatasource(typeof(IGlassBase), DatasourceNestingOptions.Always).Returns(glassModel);

			var resolvedModel = _sut.Create<AlwaysFallbackViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.Datasource);
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
				Assert.AreSame(_datasourceModel, Datasource);
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

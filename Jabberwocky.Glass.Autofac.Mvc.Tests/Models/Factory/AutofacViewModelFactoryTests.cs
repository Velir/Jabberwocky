using Autofac;
using Autofac.Core;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Mvc.Models;
using Jabberwocky.Glass.Autofac.Mvc.Models.Factory;
using Jabberwocky.Glass.Autofac.Mvc.Services;
using Jabberwocky.Glass.Models;
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
		private ISitecoreContext _sitecoreContext;
		private IRenderingContextService _renderingContextService;

		[SetUp]
		public void Setup()
		{
			_resolver = Substitute.For<IComponentContext>();
			_renderingContextService = Substitute.For<IRenderingContextService>();
			_sitecoreContext = Substitute.For<ISitecoreContext>();

			IComponentRegistration retVal;
			_resolver.ComponentRegistry.TryGetRegistration(null, out retVal).ReturnsForAnyArgs(true);

			_sut = new AutofacViewModelFactory(_resolver, _renderingContextService, _sitecoreContext);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_DirectDerivedType_ReturnsCorrectGenericType()
		{
			var genericType = AutofacViewModelFactory.InternalGetGlassModelTypeFromGenericParam(typeof(DirectViewModel));
			Assert.IsNotNull(genericType);
			Assert.AreEqual(typeof(IGlassBase), genericType);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_IndirectDerivedType_ReturnsCorrectGenericType()
		{
			var genericType = AutofacViewModelFactory.InternalGetGlassModelTypeFromGenericParam(typeof(IndirectViewModel));
			Assert.IsNotNull(genericType);
			Assert.AreEqual(typeof(IGlassBase), genericType);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_IndirectGenericType_ReturnsCorrectGenericType()
		{
			var genericType = AutofacViewModelFactory.InternalGetGlassModelTypeFromGenericParam(typeof(IndirectGenericViewModel));
			Assert.IsNotNull(genericType);
			Assert.AreEqual(typeof(IGlassBase), genericType);
		}

		[Test]
		public void GetGlassModelTypeFromGenericParam_NoInheritViewModel_ReturnsNull()
		{
			var genericType = AutofacViewModelFactory.InternalGetGlassModelTypeFromGenericParam(typeof(NoInheritViewModel));
			Assert.IsNull(genericType);
		}

		[Test]
		public void Create_InjectableGlassViewModel_SetsInternalModel()
		{
			var mockRendering = Substitute.For<Rendering>();
			_renderingContextService.GetCurrentRendering().Returns(mockRendering);

			var viewModel = new InjectableViewModel();
			var glassModel = Substitute.For<IGlassBase>();
			_resolver.ResolveOptional(typeof(object), new Parameter[0]).ReturnsForAnyArgs(viewModel);
			_sitecoreContext.GetCurrentItem<IGlassBase>(inferType: true).Returns(glassModel);

			var resolvedModel = _sut.Create<InjectableViewModel>();

			Assert.AreSame(viewModel, resolvedModel);
			Assert.AreSame(glassModel, viewModel.GlassModel);
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

		#endregion

	}
}

using System;
using Autofac;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Mvc.Services;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Autofac.Mvc.Models.Factory
{
	public class AutofacViewModelFactory : IViewModelFactory
	{
		private readonly IComponentContext _resolver;
		private readonly ISitecoreContext _context;
		private readonly IRenderingContextService _renderingContextService;

		public AutofacViewModelFactory(IComponentContext resolver, IRenderingContextService renderingContextService, ISitecoreContext context)
		{
			if (resolver == null) throw new ArgumentNullException("resolver");
			if (renderingContextService == null) throw new ArgumentNullException("renderingContextService");
			if (context == null) throw new ArgumentNullException("context");
			_resolver = resolver;
			_renderingContextService = renderingContextService;
			_context = context;
		}

		public TModel Create<TModel>() where TModel : class
		{
			return Create(typeof (TModel)) as TModel;
		}

		public object Create(Type model)
		{
			var viewModel = _resolver.Resolve(model);

			var glassViewModel = viewModel as InjectableGlassViewModelBase;
			if (glassViewModel != null)
			{
				glassViewModel.InternalModel = GetGlassModel();
			}

			return viewModel;
		}

		private IGlassBase GetGlassModel()
		{
			var rendering = _renderingContextService.GetCurrentRendering();

			if (rendering == null || string.IsNullOrEmpty(rendering.DataSource))
			{
				return _context.GetCurrentItem<IGlassBase>(inferType: true);
			}

			// Depending on if the datasource is a GUID vs Path, use the correct overload
			Guid dataSourceGuid;
			return Guid.TryParse(rendering.DataSource, out dataSourceGuid)
				? _context.GetItem<IGlassBase>(dataSourceGuid, inferType: true)
				: _context.GetItem<IGlassBase>(rendering.DataSource, inferType: true);
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Linq;
using Autofac;
using Autofac.Core;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Mvc.Services;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Autofac.Mvc.Models.Factory
{
	public class AutofacViewModelFactory : IViewModelFactory
	{
		private static readonly ConcurrentDictionary<Type, Type> ViewModelTypeCache = new ConcurrentDictionary<Type, Type>();

		private readonly IComponentContext _resolver;
		private readonly ISitecoreContext _context;
		private readonly IRenderingContextService _renderingContextService;

		public AutofacViewModelFactory(IComponentContext resolver, IRenderingContextService renderingContextService, ISitecoreContext context)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (renderingContextService == null) throw new ArgumentNullException(nameof(renderingContextService));
			if (context == null) throw new ArgumentNullException(nameof(context));
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
			var glassModel = GetGlassModel();
			var glassModelType = GetGlassModelTypeFromGenericparam(model);
			var viewModel = _resolver.ResolveOptional(model, GetModelConstructorParam(glassModelType, glassModel));

			var glassViewModel = viewModel as InjectableGlassViewModelBase;
			if (glassViewModel != null)
			{
				glassViewModel.InternalModel = glassModel;
			}

			return viewModel;
		}

		private static Parameter[] GetModelConstructorParam(Type glassModelType, IGlassBase glassModel)
		{
			return glassModel == null || glassModelType == null
				? new Parameter[0]
				: new Parameter[] { new TypedParameter(glassModelType, glassModel) };
		}

		internal static Type InternalGetGlassModelTypeFromGenericParam(Type viewModel)
		{
			if (viewModel.IsGenericType && !typeof(GlassViewModel<>).IsAssignableFrom(viewModel.GetGenericTypeDefinition()))
				return null;

			Func<Type, Type> findBaseType = null;
			findBaseType = baseType =>
			{
				var @base = baseType?.BaseType;

				if (@base != null && !@base.IsGenericType)
				{
					return findBaseType(@base);
				}

				// recursive call
				return @base?.GetGenericTypeDefinition() == typeof(GlassViewModel<>)
					? @base
					: @base == null
						? null
						: findBaseType(@base);
			};

			var baseGlassViewModelType = findBaseType(viewModel);
			return baseGlassViewModelType?.GenericTypeArguments.First();
		}

		private Type GetGlassModelTypeFromGenericparam(Type viewModel)
		{
			return ViewModelTypeCache.GetOrAdd(viewModel, InternalGetGlassModelTypeFromGenericParam);
		}

		private IGlassBase GetGlassModel()
		{
			var rendering = _renderingContextService.GetCurrentRendering();

			if (string.IsNullOrEmpty(rendering?.DataSource))
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

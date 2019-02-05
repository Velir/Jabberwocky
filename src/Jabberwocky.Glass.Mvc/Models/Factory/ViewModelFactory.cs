using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Jabberwocky.Glass.Mvc.Models.Attributes;
using Jabberwocky.Glass.Mvc.Services;

namespace Jabberwocky.Glass.Mvc.Models.Factory
{
	public class ViewModelFactory : IViewModelFactory
	{
		private static readonly ConcurrentDictionary<Type, TypeTuple?> ViewModelTypeCache = new ConcurrentDictionary<Type, TypeTuple?>();
		private readonly IServiceProvider _provider;
		private readonly IRenderingContextService _renderingContextService;
		public ViewModelFactory(IServiceProvider provider, IRenderingContextService renderingContextService)
		{
			if (renderingContextService == null) throw new ArgumentNullException(nameof(renderingContextService));
			_provider = provider;
			_renderingContextService = renderingContextService;
		}
		public TModel Create<TModel>() where TModel : class
		{
			return Create(typeof(TModel)) as TModel;
		}
		public object Create(Type model)
		{
			var glassModelType = GetGlassModelTypeFromGenericParam(model);
			var glassModel = GetGlassModel(model, glassModelType);
			var renderingParamType = GetRenderingModelTypeFromGenericParam(model);
			var renderingParamModel = GetRenderingParamModel(renderingParamType);
			var viewModel = ResolveViewModel(model, glassModelType, glassModel, renderingParamType, renderingParamModel);
			var glassViewModel = viewModel as InjectableGlassViewModelBase;
			if (glassViewModel != null)
			{
				glassViewModel.InternalDatasourceModel = glassModel;
				glassViewModel.InternalRenderingParameterModel = renderingParamModel;
			}
			return viewModel;
		}
		protected virtual object ResolveViewModel(Type model, Type glassModelType, object glassModel, Type renderingParameterType,
			object renderingParameterModel)
		{
			return _provider?.GetService(model);
		}

		internal static TypeTuple? InternalGetGlassModelAndRenderingTypesFromGenericParam(Type viewModel)
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
				var genericTypeDef = @base?.GetGenericTypeDefinition();
				return genericTypeDef == typeof(GlassViewModel<>) || genericTypeDef == typeof(GlassViewModel<,>)
									? @base
									: @base == null
											? null
											: findBaseType(@base);
			};

			var baseGlassViewModelType = findBaseType(viewModel);
			var typeArgs = baseGlassViewModelType?.GenericTypeArguments;

			return typeArgs == null
					? (TypeTuple?)null
					: new TypeTuple
					{
						GlassModel = typeArgs.First(), // required
						RenderingParamModel = typeArgs.Skip(1).FirstOrDefault() // optional
					};
		}

		private Type GetGlassModelTypeFromGenericParam(Type viewModel)
		{
			return ViewModelTypeCache.GetOrAdd(viewModel, InternalGetGlassModelAndRenderingTypesFromGenericParam)?.GlassModel;
		}

		private Type GetRenderingModelTypeFromGenericParam(Type viewModel)
		{
			return ViewModelTypeCache.GetOrAdd(viewModel, InternalGetGlassModelAndRenderingTypesFromGenericParam)?.RenderingParamModel;
		}

		private object GetRenderingParamModel(Type renderingParamType)
		{
			return renderingParamType == null
					? null
					: _renderingContextService.GetCurrentRenderingParameters(renderingParamType);
		}

		private object GetGlassModel(Type viewModelType, Type glassModelType)
		{
			if (glassModelType == null) return null;

			var datasourceConfigAttr = viewModelType.GetCustomAttribute<ConfigureDatasourceAttribute>();
			var config = DatasourceNestingOptions.Default;
			if (datasourceConfigAttr != null)
			{
				config = datasourceConfigAttr.Config == DatasourceResolution.AllowNesting
						? DatasourceNestingOptions.Always
						: DatasourceNestingOptions.Never;
			}

			return _renderingContextService.GetCurrentRenderingDatasource(glassModelType, config);
		}

		internal struct TypeTuple
		{
			public Type GlassModel;
			public Type RenderingParamModel;
		}
	}
}

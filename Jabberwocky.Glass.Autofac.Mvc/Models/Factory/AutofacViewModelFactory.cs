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
        private static readonly ConcurrentDictionary<Type, TypeTuple?> ViewModelTypeCache = new ConcurrentDictionary<Type, TypeTuple?>();

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
            return Create(typeof(TModel)) as TModel;
        }

        public object Create(Type model)
        {
            var glassModel = GetGlassModel();
            var glassModelType = GetGlassModelTypeFromGenericParam(model);
            var renderingParamType = GetRenderingModelTypeFromGenericParam(model);
            var renderingParamModel = GetRenderingParamModel(renderingParamType);
            var viewModel = _resolver.ResolveOptional(model, GetModelConstructorParams(glassModelType, glassModel, renderingParamType, renderingParamModel));

            var glassViewModel = viewModel as InjectableGlassViewModelBase;
            if (glassViewModel != null)
            {
                glassViewModel.InternalDatasourceModel = glassModel;
                glassViewModel.InternalRenderingParameterModel = renderingParamModel;
            }

            return viewModel;
        }

        private static Parameter[] GetModelConstructorParams(Type glassModelType, IGlassBase glassModel, Type renderingModelType = null, object renderingModel = null)
        {
            var parameterArray = glassModel == null || glassModelType == null
                ? new Parameter[0]
                : new Parameter[] { new TypedParameter(glassModelType, glassModel) };

            return renderingModelType == null || renderingModel == null
                ? parameterArray
                : parameterArray
                    .Concat(new[] { new TypedParameter(renderingModelType, renderingModel) })
                    .ToArray();
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

        private IGlassBase GetGlassModel()
        {
            var rendering = _renderingContextService.GetCurrentRendering();
            if (rendering == null)
            {
                throw new InvalidOperationException("This processor can only be executed in an MVC rendering context.");
            }

            if (!string.IsNullOrEmpty(rendering.DataSource))
            {
                // Depending on if the datasource is a GUID vs Path, use the correct overload
                Guid dataSourceGuid;
                return Guid.TryParse(rendering.DataSource, out dataSourceGuid)
                    ? _context.GetItem<IGlassBase>(dataSourceGuid, inferType: true)
                    : _context.GetItem<IGlassBase>(rendering.DataSource, inferType: true);
            }

            // Try to get from the Static Item
            var staticItem = rendering.Item;
            if (staticItem != null)
            {
                return _context.GetItem<IGlassBase>(staticItem.ID.Guid, inferType: true);
            }

            return _context.GetCurrentItem<IGlassBase>(inferType: true);
        }

        internal struct TypeTuple
        {
            public Type GlassModel;
            public Type RenderingParamModel;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Sitecore.Mvc.Presentation;
using Glass.Mapper.Sc;
using System.Linq;

namespace Jabberwocky.Glass.Autofac.Mvc.Services
{
	public class RenderingContextService : IRenderingContextService
	{
	    private const string RenderingParameterPropertyName = "Parameters";

	    private readonly IGlassHtml _glassHtml;

	    private static readonly ConcurrentDictionary<Type, MethodInfo> GenericMethodCache = new ConcurrentDictionary<Type, MethodInfo>();

	    public RenderingContextService(IGlassHtml glassHtml)
	    {
	        if (glassHtml == null) throw new ArgumentNullException(nameof(glassHtml));
	        _glassHtml = glassHtml;
	    }

	    public Rendering GetCurrentRendering()
		{
			var context = RenderingContext.CurrentOrNull;

			return context?.Rendering;
		}

	    public T GetCurrentRenderingParameters<T>() where T : class
	    {
	        var parameterString = RenderingContext.CurrentOrNull?.Rendering?[RenderingParameterPropertyName];

            return string.IsNullOrEmpty(parameterString) 
                ? null 
                : _glassHtml.GetRenderingParameters<T>(parameterString);
	    }

	    public object GetCurrentRenderingParameters(Type renderingParamType)
	    {
	        var genericThunk = GenericMethodCache.GetOrAdd(renderingParamType, type =>
	        {
	            return typeof (RenderingContextService).GetMethods(BindingFlags.Public | BindingFlags.Instance)
	                .First(method => method.Name == nameof(RenderingContextService.GetCurrentRenderingParameters) && method.IsGenericMethodDefinition);
	        });

	        return genericThunk.Invoke(this, null);
	    }
	}
}

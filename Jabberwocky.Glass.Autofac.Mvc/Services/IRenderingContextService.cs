using System;
using Jabberwocky.Glass.Models;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Autofac.Mvc.Services
{
	public interface IRenderingContextService
	{
		Rendering GetCurrentRendering();

	    T GetCurrentRenderingDatasource<T>(DatasourceNestingOptions options = DatasourceNestingOptions.Default) where T : class, IGlassBase;

        T GetCurrentRenderingParameters<T>() where T : class;

	    object GetCurrentRenderingParameters(Type renderingParamType);
	}

    public enum DatasourceNestingOptions
    {
        Default,
        Never,
        Always
    }
}

using System;
using Jabberwocky.Glass.Models;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Mvc.Services
{
	public interface IRenderingContextService
	{
		Rendering GetCurrentRendering();

		T GetCurrentRenderingDatasource<T>(DatasourceNestingOptions options = DatasourceNestingOptions.Default) where T : class;

		object GetCurrentRenderingDatasource(Type modelType, DatasourceNestingOptions options = DatasourceNestingOptions.Default);

		T GetCurrentRenderingParameters<T>() where T : class;

		object GetCurrentRenderingParameters(Type renderingParamType);
	}
}
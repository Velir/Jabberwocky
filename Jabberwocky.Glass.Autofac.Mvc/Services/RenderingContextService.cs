using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Autofac.Mvc.Services
{
	public class RenderingContextService : IRenderingContextService
	{
		public Rendering GetCurrentRendering()
		{
			var context = RenderingContext.CurrentOrNull;

			return context == null
				? null
				: context.Rendering;
		}
	}
}

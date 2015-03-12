﻿using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Glass.Autofac.Mvc.Services
{
	public interface IRenderingContextService
	{
		//TODO: Rendering looks testable, but evaluate if we should return this directly or not
		Rendering GetCurrentRendering();
	}
}

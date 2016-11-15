using System;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory.Caching
{
	public interface IGlassTemplateCacheService
	{
		Type GetImplementingTypeForItem(IGlassCore item, Type interfaceType);
		Type GetImplementingTypeForTemplate(Guid templateId, Type interfaceType);
		Type GetFallbackImplementingTypeForTemplate(Guid templateId, Type interfaceType);
	}
}

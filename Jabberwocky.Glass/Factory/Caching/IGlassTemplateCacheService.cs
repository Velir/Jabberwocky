using System;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory.Caching
{
	public interface IGlassTemplateCacheService
	{
		Type GetImplementingTypeForItem(IGlassBase item, Type interfaceType);
	}
}

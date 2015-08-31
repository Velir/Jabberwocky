using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Factory.Caching.Providers
{
	public interface IGlassTypesLoader
	{
		ILookup<Type, GlassInterfaceMetadata> LoadImplementations(IEnumerable<string> assemblyNames);
	}
}

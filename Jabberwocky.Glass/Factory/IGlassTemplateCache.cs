using System;
using System.Collections.Generic;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Factory
{
	public interface IGlassTemplateCache
	{
		IDictionary<Type, IDictionary<string, Type>> TemplateCache { get; }

		IEnumerable<Guid> GetBaseTemplates(IBaseTemplates item, ISitecoreService service, int depth = 2);
	}
}

using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Jabberwocky.Glass.Factory.Util
{
	[SitecoreType]
	public interface IBaseTemplates
	{
		// This is for content items
		[SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
		IEnumerable<Guid> BaseTemplates { get; }

		// Use this for template items
		[SitecoreField("__Base template")]
		IEnumerable<Guid> TemplateBaseTemplates { get; }

		[SitecoreInfo(SitecoreInfoType.TemplateId)]
		Guid Template { get; }
	}
}

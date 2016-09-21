using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Globalization;

namespace Jabberwocky.Glass.Models
{
	public interface IGlassBase
	{
		[SitecoreId]
		Guid _Id { get; set; }

		[SitecoreInfo(SitecoreInfoType.Language)]
		Language _Language { get; set; }

		[SitecoreInfo(SitecoreInfoType.Version)]
		int _Version { get; set; }

		[SitecoreInfo(SitecoreInfoType.Url)]
		string _Url { get; set; }

		[SitecoreInfo(SitecoreInfoType.TemplateId)]
		Guid _TemplateId { get; set; }

		[SitecoreInfo(SitecoreInfoType.Path)]
		string _Path { get; set; }

		[SitecoreInfo(SitecoreInfoType.Name)]
		string _Name { get; set; }

		[SitecoreChildren(InferType = true, IsLazy = true)]
		IEnumerable<IGlassBase> _ChildrenWithInferType { get; set; }

		[SitecoreParent(InferType = true, IsLazy = true)]
		IGlassBase _Parent { get; set; }

		[SitecoreInfo(SitecoreInfoType.MediaUrl)]
		string _MediaUrl { get; set; }

		[SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
		IEnumerable<Guid> _BaseTemplates { get; set; }
	}
}

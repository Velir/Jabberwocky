using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Globalization;

namespace Jabberwocky.Glass.Models
{
	public interface IGlassBase : IGlassCore, IGlassRelationships<IGlassBase>, IGlassExtendedAttributes
	{
		
	}

	public interface IGlassExtendedAttributes
	{
		[SitecoreInfo(SitecoreInfoType.Url)]
		string _Url { get; set; }

		[SitecoreInfo(SitecoreInfoType.Name)]
		string _Name { get; set; }

		[SitecoreInfo(SitecoreInfoType.MediaUrl)]
		string _MediaUrl { get; set; }

        [SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
        IEnumerable<Guid> _BaseTemplates { get; set; }
    }

	public interface IGlassCore
	{
		[SitecoreId]
		Guid _Id { get; set; }

		[SitecoreInfo(SitecoreInfoType.Language)]
		Language _Language { get; set; }

		[SitecoreInfo(SitecoreInfoType.TemplateId)]
		Guid _TemplateId { get; set; }

		[SitecoreInfo(SitecoreInfoType.Path)]
		string _Path { get; set; }

		[SitecoreInfo(SitecoreInfoType.Version)]
		int _Version { get; set; }
	}

	public interface IGlassRelationships<T> where T : IGlassCore
	{
		[SitecoreChildren(InferType = true)]
		IEnumerable<T> _ChildrenWithInferType { get; set; }

		[SitecoreParent(InferType = true)]
		T _Parent { get; set; }
	}
}

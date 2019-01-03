using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Globalization;

namespace Jabberwocky.Glass.Models
{
	public class GlassBase : IGlassBase
	{
		[SitecoreId]
		public virtual Guid _Id { get; set; }

		[SitecoreInfo(SitecoreInfoType.Language)]
		public virtual Language _Language { get; set; }

		[SitecoreInfo(SitecoreInfoType.Version)]
		public virtual int _Version { get; set; }

		[SitecoreInfo(SitecoreInfoType.Url)]
		public virtual string _Url { get; set; }

		[SitecoreInfo(SitecoreInfoType.TemplateId)]
		public virtual Guid _TemplateId { get; set; }

		[SitecoreInfo(SitecoreInfoType.Path)]
		public virtual string _Path { get; set; }

		[SitecoreInfo(SitecoreInfoType.Name)]
		public virtual string _Name { get; set; }

		[SitecoreChildren(InferType = true)]
		public virtual IEnumerable<IGlassBase> _ChildrenWithInferType { get; set; }

		[SitecoreParent]
		public virtual IGlassBase _Parent { get; set; }

		[SitecoreInfo(SitecoreInfoType.MediaUrl)]
		public virtual string _MediaUrl { get; set; }

		[SitecoreInfo(SitecoreInfoType.BaseTemplateIds)]
		public virtual IEnumerable<Guid> _BaseTemplates { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Models;
using Sitecore;
using Sitecore.Data.Items;

namespace Jabberwocky.Glass.Services
{
	public class LinkService : ILinkService
	{
		private readonly ISitecoreService _service;

		public LinkService(ISitecoreService service)
		{
			if (service == null) throw new ArgumentNullException(nameof(service));
			_service = service;
		}

		public IEnumerable<IGlassCore> GetReferrers(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);

			var links = Globals.LinkDatabase.GetReferrers(item);
			var linkReferences = links.Select(i => _service.GetItem<IGlassCore>(i.SourceItemID.Guid, i.SourceItemLanguage, inferType: true)).Where(i => i != null);
			return linkReferences;
		}

		public IEnumerable<IGlassCore> GetValidLinkTargets(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);

			var links = item.Links.GetValidLinks();
			return links.Select(link => link.GetTargetItem().GlassCast<IGlassCore>(inferType: true));
		}
	}
}

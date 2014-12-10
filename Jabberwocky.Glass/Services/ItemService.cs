using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Models;
using Sitecore.Data.Items;

namespace Jabberwocky.Glass.Services
{
	public class ItemService : IItemService
	{
		private readonly ISitecoreService _service;

		public ItemService(ISitecoreService service)
		{
			if (service == null) throw new ArgumentNullException("service");
			_service = service;
		}

		public IEnumerable<IGlassBase> GetDescendants(IGlassBase glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);
			return item.Axes.GetDescendants().Select(sItem => _service.GetItem<IGlassBase>(sItem.ID.Guid, inferType: true));
		}

		public IEnumerable<IGlassBase> GetAncestors(IGlassBase glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);
			return item.Axes.GetAncestors().Select(sItem => _service.GetItem<IGlassBase>(sItem.ID.Guid, inferType: true));
		}

		public bool HasPresentation(IGlassBase glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);
			return item != null && item[Sitecore.FieldIDs.LayoutField] != string.Empty;
		}

		public bool IsContentItem(IGlassBase glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);
			return item != null && item.Paths.IsContentItem;
		}

		public bool IsMediaItem(IGlassBase glassItem)
		{
			var item = _service.GetItem<Item>(glassItem._Id, glassItem._Language);
			return item != null && item.Paths.IsMediaItem;
		}
	}
}

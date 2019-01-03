﻿using System;
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
			if (service == null) throw new ArgumentNullException(nameof(service));
			_service = service;
		}

		public IEnumerable<IGlassCore> GetDescendants(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(new GetItemByIdOptions
			{
				Id = glassItem._Id,
				Language = glassItem._Language
			});
			return item.Axes.GetDescendants().Select(sItem => _service.GetItem<IGlassCore>(sItem, x => x.InferType()));
		}

		public IEnumerable<IGlassCore> GetAncestors(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(new GetItemByIdOptions
			{
				Id = glassItem._Id,
				Language = glassItem._Language
			});
			return item.Axes.GetAncestors().Select(sItem => _service.GetItem<IGlassCore>(sItem, x => x.InferType()));
		}

		public bool HasPresentation(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(new GetItemByIdOptions
			{
				Id = glassItem._Id,
				Language = glassItem._Language
			});
			return item != null && item[Sitecore.FieldIDs.FinalLayoutField] != string.Empty;
		}

		public bool IsContentItem(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(new GetItemByIdOptions
			{
				Id = glassItem._Id,
				Language = glassItem._Language
			});
			return item != null && item.Paths.IsContentItem;
		}

		public bool IsMediaItem(IGlassCore glassItem)
		{
			var item = _service.GetItem<Item>(new GetItemByIdOptions
			{
				Id = glassItem._Id,
				Language = glassItem._Language
			});
			return item != null && item.Paths.IsMediaItem;
		}
	}
}

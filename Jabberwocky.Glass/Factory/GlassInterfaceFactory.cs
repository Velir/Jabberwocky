using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory
{
	public class GlassInterfaceFactory : IGlassInterfaceFactory
	{
		private readonly IGlassTemplateCacheService _templateCache;
		private readonly IImplementationFactory _factory;
		private readonly Func<ISitecoreService> _serviceFactory;

		public IDictionary<Type, IDictionary<string, Type>> TemplateCache => _templateCache.TemplateCache;

		/// <summary>
		/// Gets the template cache service used by this instance
		/// </summary>
		internal IGlassTemplateCacheService TemplateCacheService => _templateCache;

		public GlassInterfaceFactory(IGlassTemplateCacheService templateCache, IImplementationFactory factory, Func<ISitecoreService> serviceFactory)
		{
			if (templateCache == null) throw new ArgumentNullException(nameof(templateCache));
			if (factory == null) throw new ArgumentNullException(nameof(factory));
			if (serviceFactory == null) throw new ArgumentNullException(nameof(serviceFactory));

			_templateCache = templateCache;
			_factory = factory;
			_serviceFactory = serviceFactory;
		}

		public T GetItem<T>(IGlassBase model) where T : class
		{
			if (TemplateCache.ContainsKey(typeof(T)))
			{
				var implType = GetItemFromInterface(model, typeof(T));

				return implType != null
					? _factory.Create<T, IGlassBase>(implType, model)
					: default(T);
			}
			return default(T);
		}

		public IEnumerable<T> GetItems<T>(IEnumerable<IGlassBase> models) where T : class
		{
			if (models == null) return Enumerable.Empty<T>();

			return models.Select(GetItem<T>).Where(x => x != default(T));
		}

		private Type GetItemFromInterface(IGlassBase item, Type interfaceType)	 // interfaceType: this will be non-generic type, ie. IListable
		{
			if (item == null || interfaceType == null) return null;

			if (!TemplateCache.ContainsKey(interfaceType))
			{
				return null;
			}

			var itemInterfaces = TemplateCache[interfaceType];
			var itemTemplateId = item._TemplateId.ToString();

			if (itemInterfaces.ContainsKey(itemTemplateId))
			{
				// We found an exact match...
				return itemInterfaces[itemTemplateId];
			}

			// No exact match... Try base templates (up to configurable depth)
			using (ISitecoreService service = _serviceFactory())
			{
				foreach (Guid baseTemplateId in _templateCache.GetBaseTemplates(service.GetItem<IBaseTemplates>(item._Id), service))
				{
					string templateId = baseTemplateId.ToString();
					if (itemInterfaces.ContainsKey(templateId))
					{
						return itemInterfaces[templateId];
					}
				}
			}

			return null;
		}
	}
}

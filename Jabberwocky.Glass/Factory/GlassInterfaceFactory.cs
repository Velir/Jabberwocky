using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory
{
	public class GlassInterfaceFactory : IGlassInterfaceFactory, IGlassTemplateCache
	{
		private readonly IImplementationFactory _factory;
		private readonly Func<ISitecoreService> _serviceFactory;
		private readonly IDictionary<Type, IDictionary<string, Type>> _templateCache;

		private static readonly string DefaultFallbackTemplateId = Guid.Empty.ToString();
		private static readonly Guid[] DefaultBaseTemplateArray = { Guid.Empty };

		public IDictionary<Type, IDictionary<string, Type>> TemplateCache { get { return _templateCache; } }

		/// <summary>
		/// Maximum search depth for base-template traversal
		/// </summary>
		private const int MaxDepth = 2;

		public GlassInterfaceFactory(ILookup<Type, GlassInterfaceMetadata> interfaceMappings, IImplementationFactory factory, Func<ISitecoreService> serviceFactory)
		{
			if (interfaceMappings == null) throw new ArgumentNullException("interfaceMappings");
			if (factory == null) throw new ArgumentNullException("factory");
			if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");

			_factory = factory;
			_serviceFactory = serviceFactory;
			_templateCache = GenerateCache(interfaceMappings);
		}

		public T GetItem<T>(IGlassBase model) where T : class
		{
			if (_templateCache.ContainsKey(typeof(T)))
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

			if (TemplateCache == null || !TemplateCache.ContainsKey(interfaceType))
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
			ISitecoreService service = _serviceFactory();
			foreach (Guid baseTemplateId in GetBaseTemplates(service.GetItem<IBaseTemplates>(item._Id), service))
			{
				string templateId = baseTemplateId.ToString();
				if (itemInterfaces.ContainsKey(templateId))
				{
					return itemInterfaces[templateId];
				}
			}

			return null;
		}

		public IEnumerable<Guid> GetBaseTemplates(IBaseTemplates item, ISitecoreService service, int depth = MaxDepth)
		{
			if (item == null || depth <= 0) return DefaultBaseTemplateArray;

			var baseTemplates = item.TemplateBaseTemplates ?? item.BaseTemplates;
			if (baseTemplates == null || !baseTemplates.Any()) return DefaultBaseTemplateArray;

			// Breadth first search (recursive)
			return baseTemplates.Concat(baseTemplates.SelectMany(guid => GetBaseTemplates(service.GetItem<IBaseTemplates>(guid), service, depth - 1)));
		}

		private IDictionary<Type, IDictionary<string, Type>> GenerateCache(ILookup<Type, GlassInterfaceMetadata> interfaceMappings)
		{
			var dictionary = new Dictionary<Type, IDictionary<string, Type>>();

			foreach (var mappingGroup in interfaceMappings)
			{
				var innerDictionary = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

				// Key: Factory Interface type
				dictionary.Add(mappingGroup.Key, innerDictionary);

				foreach (var metadata in mappingGroup)
				{
					var sitecoreAttribute = metadata.GlassType.GetCustomAttributes(typeof(SitecoreTypeAttribute), false).FirstOrDefault() as SitecoreTypeAttribute;
					var templateId = metadata.IsFallback
							? DefaultFallbackTemplateId
							: sitecoreAttribute == null ? null : sitecoreAttribute.TemplateId;

					if (!string.IsNullOrEmpty(templateId))
					{
						innerDictionary.Add(templateId, metadata.ImplementationType);
					}
				}
			}

			return dictionary;
		}
	}
}

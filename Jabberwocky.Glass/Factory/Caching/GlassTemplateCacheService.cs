using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory.Caching
{
	/// <summary>
	/// Discovers and caches mappings between Glass Factory Interfaces to concrete implementations
	/// </summary>
	/// <remarks>
	/// This class is thread-safe
	/// </remarks>
	public class GlassTemplateCacheService : IGlassTemplateCacheService
	{
		/// <summary>
		/// Default search depth for base-template traversal
		/// </summary>
		private const int DefaultDepth = 2;
		/// <summary>
		/// Maximum search depth for base-template traversal
		/// </summary>
		private const int MaxDepth = 100;
		private static readonly string DefaultFallbackTemplateId = Guid.Empty.ToString();
		private static readonly Guid[] DefaultBaseTemplateArray = new Guid[0];

		private readonly Func<ISitecoreService> _serviceFactory;
		private readonly ConcurrentDictionary<Tuple<Type, string>, Type> _firstLevelCache = new ConcurrentDictionary<Tuple<Type, string>, Type>();

		/// <summary>
		/// 'Static' cache of Glass Factory Interface types to corresponding mapping of Template -> Implementations
		/// </summary>
		protected internal IDictionary<Type, IDictionary<string, GlassInterfaceMetadata>> TemplateCache { get; }


		public GlassTemplateCacheService(ILookup<Type, GlassInterfaceMetadata> interfaceMappings, Func<ISitecoreService> serviceFactory)
		{
			if (serviceFactory == null) throw new ArgumentNullException(nameof(serviceFactory));
			_serviceFactory = serviceFactory;

			TemplateCache = GenerateCache(interfaceMappings);
		}

		public Type GetImplementingTypeForItem(IGlassBase item, Type interfaceType)
		{
			return GetImplementingTypeForTemplate(item._TemplateId, interfaceType);
		}

		public Type GetImplementingTypeForTemplate(Guid templateId, Type interfaceType)
		{
			if (!TemplateCache.ContainsKey(interfaceType))
			{
				return null;
			}

			return InnerGetImplementingTypeForTemplate(templateId, interfaceType, MaxDepth);
		}

		private Type InnerGetImplementingTypeForTemplate(Guid templateId, Type interfaceType, int depth)
		{
			var templateKey = templateId.ToString();

			var itemInterfaces = TemplateCache[interfaceType];
			if (itemInterfaces.ContainsKey(templateKey))
			{
				// We found an exact match...
				return itemInterfaces[templateKey].ImplementationType;
			}

			// If no exact match exists, attempt to resolve from 1st-level cache
			return _firstLevelCache.GetOrAdd(new Tuple<Type, string>(interfaceType, templateKey), key =>
			{
				// Otherwise, search for match, and update 1st-level cache
				using (var service = _serviceFactory())
				{
					foreach (Guid baseTemplateId in GetBaseTemplates(service.GetItem<IBaseTemplates>(templateId), service, depth))
					{
						string templateIdString = baseTemplateId.ToString();
						if (itemInterfaces.ContainsKey(templateIdString))
						{
							return itemInterfaces[templateIdString].ImplementationType;
						}
					}
				}

				return null;
			});
		}

		public Type GetFallbackImplementingTypeForTemplate(Guid templateId, Type interfaceType)
		{
			if (!TemplateCache.ContainsKey(interfaceType))
			{
				return null;
			}

			using (var service = _serviceFactory())
			{
				var currentTemplate = service.GetItem<IBaseTemplates>(templateId);

				return GetBaseTemplates(currentTemplate, service, depth: MaxDepth)
					.Select(guid => InnerGetImplementingTypeForTemplate(guid, interfaceType, 1))
					.FirstOrDefault(type => type != null);
			}
		}

		internal IEnumerable<Guid> GetBaseTemplates(IBaseTemplates item, ISitecoreService service, int depth = DefaultDepth, bool ignoreTemplate = false)
		{
			return InternalGetBaseTemplates(item, service, new HashSet<Guid>(), depth, ignoreTemplate).Concat(new[] { new Guid(DefaultFallbackTemplateId) });
		}

		private IEnumerable<Guid> InternalGetBaseTemplates(IBaseTemplates item, ISitecoreService service, HashSet<Guid> visitedSet, int depth, bool ignoreTemplate = false)
		{
			if (item == null || depth <= 0) return DefaultBaseTemplateArray;

			// Check direct 'template' field, then base-templates, before finally recursing
			var baseTemplates = new[] { item.Template }
				.Concat((item.TemplateBaseTemplates ?? item.BaseTemplates) ?? Enumerable.Empty<Guid>())
				.Where(id => !visitedSet.Contains(id))
				.ToArray();
			
			foreach (var id in baseTemplates)
			{
				// prevent traversal of cycles
				visitedSet.Add(id);
			}

			// Breadth first search (recursive)
			return baseTemplates.Concat(baseTemplates.SelectMany(guid => InternalGetBaseTemplates(service.GetItem<IBaseTemplates>(guid), service, visitedSet, depth - 1)));
		}

		private IDictionary<Type, IDictionary<string, GlassInterfaceMetadata>> GenerateCache(ILookup<Type, GlassInterfaceMetadata> interfaceMappings)
		{
			var dictionary = new Dictionary<Type, IDictionary<string, GlassInterfaceMetadata>>();

			foreach (var mappingGroup in interfaceMappings)
			{
				var innerDictionary = new Dictionary<string, GlassInterfaceMetadata>(StringComparer.InvariantCultureIgnoreCase);

				// Key: Factory Interface type
				dictionary.Add(mappingGroup.Key, innerDictionary);

				foreach (var metadata in mappingGroup)
				{
					var sitecoreAttribute = metadata.GlassType.GetCustomAttributes(typeof(SitecoreTypeAttribute), false).FirstOrDefault() as SitecoreTypeAttribute;
					var templateId = metadata.IsFallback
							? DefaultFallbackTemplateId
							: sitecoreAttribute?.TemplateId;

					if (!string.IsNullOrEmpty(templateId))
					{
						if (innerDictionary.ContainsKey(templateId))
						{
							var implementation = innerDictionary[templateId];
							if (metadata.ZIndex > implementation.ZIndex)
							{
								innerDictionary[templateId] = metadata;
							}
						}
						else
						{
							innerDictionary.Add(templateId, metadata);
						}
					}
				}
			}

			return dictionary;
		}
	}
}

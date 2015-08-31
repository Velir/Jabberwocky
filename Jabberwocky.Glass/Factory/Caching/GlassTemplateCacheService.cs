using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Factory.Caching
{
	public class GlassTemplateCacheService : IGlassTemplateCacheService
	{
		/// <summary>
		/// Maximum search depth for base-template traversal
		/// </summary>
		private const int MaxDepth = 2;
		private static readonly string DefaultFallbackTemplateId = Guid.Empty.ToString();
		private static readonly Guid[] DefaultBaseTemplateArray = new Guid[0];

		public IDictionary<Type, IDictionary<string, Type>> TemplateCache { get; }

		public GlassTemplateCacheService(ILookup<Type, GlassInterfaceMetadata> interfaceMappings)
		{
			TemplateCache = GenerateCache(interfaceMappings);
		}

		public IEnumerable<Guid> GetBaseTemplates(IBaseTemplates item, ISitecoreService service, int depth = MaxDepth)
		{
			return InternalGetBaseTemplates(item, service, depth).Concat(new[] { new Guid(DefaultFallbackTemplateId) });
		}

		private IEnumerable<Guid> InternalGetBaseTemplates(IBaseTemplates item, ISitecoreService service, int depth)
		{
			if (item == null || depth <= 0) return DefaultBaseTemplateArray;

			var baseTemplates = (item.TemplateBaseTemplates ?? item.BaseTemplates)?.ToArray();
			if (baseTemplates == null || !baseTemplates.Any()) return DefaultBaseTemplateArray;

			// Breadth first search (recursive)
			return baseTemplates.Concat(baseTemplates.SelectMany(guid => InternalGetBaseTemplates(service.GetItem<IBaseTemplates>(guid), service, depth - 1)));
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

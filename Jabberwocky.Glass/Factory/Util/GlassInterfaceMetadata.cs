using System;

namespace Jabberwocky.Glass.Factory.Util
{
	public class GlassInterfaceMetadata
	{
		/// <summary>
		/// The abstract class implementation
		/// </summary>
		/// <returns></returns>
		public Type ImplementationType { get; internal set; }

		/// <summary>
		/// The Glass Mapper model type (template)
		/// </summary>
		/// <returns></returns>
		public Type GlassType { get; internal set; }

		/// <summary>
		/// Whether or not this abstract implementation type represents the default fallback implementation
		/// </summary>
		/// <returns></returns>
		public bool IsFallback { get; internal set; }

		/// <summary>
		/// The priority for this abstract implementation type to determine if it will be used when multiple implementations for this GlassType exist.
		/// </summary>
		/// <returns></returns>
		public int ZIndex { get; internal set; }
	}
}

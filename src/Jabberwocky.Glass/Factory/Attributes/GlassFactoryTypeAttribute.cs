using System;

namespace Jabberwocky.Glass.Factory.Attributes
{
	/// <summary>
	/// Attribute used on Glass Model classes to identify the representative Glass interface
	/// the model class is providing implemenations for.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class GlassFactoryTypeAttribute : Attribute
	{
		public Type Type { get; set; }
		public bool IsFallback { get; set; }
		public int ZIndex { get; set; }

		public GlassFactoryTypeAttribute(Type type)
		{
			Type = type;
			ZIndex = 0;
		}
	}
}

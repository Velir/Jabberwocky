using System;

namespace Jabberwocky.DependencyInjection.Autowire.Attributes
{
	public enum LifetimeScope
	{
		/// <summary>
		/// Transient behavior; instance per dependency
		/// </summary>
		Default,
		/// <summary>
		/// Instance per parent resolution scope
		/// </summary>
		PerScope,
		/// <summary>
		/// Singleton
		/// </summary>
		SingleInstance
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class AutowireServiceAttribute : Attribute
	{
		public LifetimeScope LifetimeScope { get; set; }
		public bool RegisterAsSelf { get; set; } = true;

		public AutowireServiceAttribute(LifetimeScope scope = LifetimeScope.Default)
		{
			LifetimeScope = scope;
		}
	}
}

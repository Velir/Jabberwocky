using System;

namespace Jabberwocky.Glass.Autofac.Attributes
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
		///  For Web-based scenarios; instance per web request
		/// </summary>
		PerRequest,	// TODO: Do we need this (ie, can this be covered by 'PerScope')? Might be beneficial in some cases...
		/// <summary>
		/// Externally owned; will not be tracked by the container
		/// </summary>
		NoTracking,
		/// <summary>
		/// Singleton
		/// </summary>
		SingleInstance
	}

	public class AutowireServiceAttribute : Attribute
	{
		public LifetimeScope LifetimeScope { get; set; }
		public bool IsAggregateService { get; set; }

		public AutowireServiceAttribute(LifetimeScope scope = LifetimeScope.Default, bool isAggregateService = false)
		{
			LifetimeScope = scope;
			IsAggregateService = isAggregateService;
		}

		public AutowireServiceAttribute(bool isAggregateService) 
			: this(LifetimeScope.Default, isAggregateService)
		{
		}
	}
}

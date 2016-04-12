using System;
using System.Runtime.CompilerServices;
using Autofac;
using Jabberwocky.Autofac.Internals;
#pragma warning disable 612

// ReSharper disable once CheckNamespace
namespace Jabberwocky.Glass.Autofac.Attributes
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.LifetimeScopeType)]
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
        PerRequest,
        /// <summary>
        /// Externally owned; will not be tracked by the container
        /// </summary>
        NoTracking,
        /// <summary>
        /// Singleton
        /// </summary>
        SingleInstance
    }

    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.AutowireServiceAttributeType)]
    public class AutowireServiceAttribute : Jabberwocky.Autofac.Attributes.AutowireServiceAttribute
    {
        public new LifetimeScope LifetimeScope
        {
            get
            {
                return (LifetimeScope) base.LifetimeScope;
            }
            set
            {
                base.LifetimeScope = (Jabberwocky.Autofac.Attributes.LifetimeScope) value;
            }
        }

        public AutowireServiceAttribute(LifetimeScope scope = LifetimeScope.Default, bool isAggregateService = false)
            : base((Jabberwocky.Autofac.Attributes.LifetimeScope)scope, isAggregateService)
        {
        }

        public AutowireServiceAttribute(bool isAggregateService)
            : this(LifetimeScope.Default, isAggregateService)
        {
        }
    }
}

namespace Jabberwocky.Glass.Autofac.Extensions
{
    [Obsolete]
    [TypeForwardedFrom(AssemblyConstants.AutowireRegistrationExtensionsType)]
    public static class AutowireRegistrationExtensions
    {
        public static void AutowireDependencies(this ContainerBuilder builder, bool preserveDefaults = false,
            params string[] assemblyNames)
        {
            Jabberwocky.Autofac.Extensions.AutowireRegistrationExtensions.AutowireDependencies(builder, preserveDefaults, assemblyNames);
        }
    }
}

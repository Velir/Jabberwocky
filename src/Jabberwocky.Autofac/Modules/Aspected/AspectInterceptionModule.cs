using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Aspects.Attributes;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;
using Jabberwocky.Autofac.Modules.Aspected.Strategies;
using Jabberwocky.Core.Utils.Reflection;
using Module = Autofac.Module;

namespace Jabberwocky.Autofac.Modules.Aspected
{
	public class AspectInterceptionModule : Module
	{
		protected const string ReservedProxyInterface = "Castle.DynamicProxy.IProxyTargetAccessor";
		private static readonly Lazy<IProxyStrategy[]> LazyDefaultStrategies = new Lazy<IProxyStrategy[]>(InitDefaultStrategies);

		protected static IProxyStrategy[] DefaultStrategies => LazyDefaultStrategies.Value;

		protected readonly IInterceptor[] Interceptors;
		protected readonly ICollection<IProxyStrategy> Strategies;
		protected readonly ISet<Type> IncludeTypes;
		protected readonly ISet<string> IncludeNamespaces;
		protected readonly ISet<string> ExcludeNamespaces;
		protected readonly ISet<string> ExcludeTypes;
		protected readonly ISet<string> ExcludeAssemblies;

		public AspectInterceptionModule(IInterceptor[] interceptors, params string[] assemblies)
			: this(new AspectConfiguration(interceptors, assemblies))
		{
		}

		public AspectInterceptionModule(IInterceptor[] interceptors, params IProxyStrategy[] strategies)
			: this(new AspectConfiguration(interceptors, null, strategies))
		{
		}

		public AspectInterceptionModule(IInterceptor[] interceptors, IEnumerable<IProxyStrategy> strategies, params string[] assemblies)
			: this(new AspectConfiguration(interceptors, assemblies, strategies))
		{
		}

		public AspectInterceptionModule(AspectConfiguration config)
		{
			Interceptors = config.Interceptors.ToArray();
			Strategies = config.Strategies.Concat(DefaultStrategies).ToArray();
			IncludeNamespaces = new HashSet<string>(config.IncludeNamespaces, StringComparer.InvariantCultureIgnoreCase);
			ExcludeNamespaces = new HashSet<string>(config.ExcludeNamespaces, StringComparer.InvariantCultureIgnoreCase);
			ExcludeTypes = new HashSet<string>(config.ExcludeTypes, StringComparer.InvariantCultureIgnoreCase);
			ExcludeAssemblies = new HashSet<string>(config.ExcludeAssemblies, StringComparer.InvariantCultureIgnoreCase);

			IncludeTypes = config.AutowireAspect
				? new HashSet<Type>(
					config.Assemblies.Select(LoadAssembly)
						.Where(assembly => assembly != null)
						.SelectMany(a => a.ExportedTypes)
						.Where(t => t?.GetCustomAttributeSafe<AspectAttribute>() != null))
				: new HashSet<Type>();
		}

		protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry,
			IComponentRegistration registration)
		{
			InterceptRegistration(componentRegistry, registration, Interceptors);
			
			base.AttachToComponentRegistration(componentRegistry, registration);
		}

		/// <summary>
		///     Intercept a specific component registrations.
		/// </summary>
		/// <param name="componentRegistry"></param>
		/// <param name="registration">Component registration</param>
		/// <param name="interceptors">List of interceptors to apply.</param>
		protected virtual void InterceptRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration, params IInterceptor[] interceptors)
		{
			var proxyTypes = new HashSet<Type>(interceptors.Select(i => i.GetType()));

			// proxy does not get along well with Activated event and registrations with Activated events cannot be proxied.
			// They are casted to LimitedType in the IRegistrationBuilder OnActivated method. This is the offending Autofac code:
			// 
			// public IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> OnActivated(Action<IActivatedEventArgs<TLimit>> handler)
			var handlers = GetActivatedEventHandlers(registration);
			if (handlers.Any(h => (handlers[0]?.Method?.DeclaringType?.Namespace ?? string.Empty).StartsWith("Autofac")))
			{
				return;
			}

			var type = registration.Activator.LimitType;
			var componentServices = registration.Services.OfType<IServiceWithType>().ToArray();

			Debug.Assert(type != null);

			Predicate<Type> isExplicitlyIncludedType = t => IncludeTypes.Any() && !IncludeTypes.Contains(t);
			Predicate<IEnumerable<IServiceWithType>> allServicesAreIncludedInNamespaces = services =>
				services.All(swt => IncludeNamespaces.Any(ns => swt.ServiceType.IsInNamespace(ns)));

			// Short circuit if any service type is explicitly registered via 'includeNamespaces' or 'includeTypes'
			if (!isExplicitlyIncludedType(type)
				&& (!allServicesAreIncludedInNamespaces(componentServices)
					|| componentServices // Only proxy classes/INTERFACES that are VISIBLE
						.All(swt => !swt.ServiceType.IsVisible)))
			{
				return;
			}

			// prevent proxying the known proxies; doesn't handle other proxies
			if (proxyTypes.Contains(type)
				|| componentServices.Any(swt => ExcludeTypes.Contains(swt.ServiceType.FullName)
				|| ExcludeNamespaces.Any(ns => swt.ServiceType.IsInNamespace(ns)))
				|| ExcludeAssemblies.Contains(type.Assembly.GetName().Name)
				|| type.IsAssignableTo<IEnumerable>()) // prevent IEnumerable/Collection proxies
			{
				return;
			}

			var proxiedInterfaces =
				type.GetInterfaces()
					.Where(i => i.IsVisible && !i.FullName.Equals(ReservedProxyInterface))
					.ToArray();

			var context = new InterceptionContext
			{
				ComponentRegistration = registration,
				ComponentRegistry = componentRegistry,
				Interceptors = interceptors,
				ComponentServices = componentServices,
				ProxyableInterfaces = proxiedInterfaces,
				InstanceType = type
			};

			ExecuteProxyStrategy(context);
		}

		protected virtual void ExecuteProxyStrategy(InterceptionContext context)
		{
			// Select an appropriate strategy
			var strategy = Strategies.FirstOrDefault(strat => strat.CanHandle(context));
			strategy?.CreateProxy(context);
		}

		#region Private Helpers

		private static IProxyStrategy[] InitDefaultStrategies()
		{
			return new IProxyStrategy[]
			{
				new InterfaceProxyStrategy(),
				new ClassProxyStrategy(),
			};
		}

		/// <summary>
		///     Get Activated event handlers for a registrations
		/// </summary>
		/// <param name="registration">Registration to retrieve events from</param>
		/// <returns>Array of delegates in the event handler</returns>
		private static Delegate[] GetActivatedEventHandlers(IComponentRegistration registration)
		{
			var eventHandlerField = GetRegistrationActivatedField(registration.GetType());
			var registrations = eventHandlerField.GetValue(registration);
			return registrations == null
				? new Delegate[0]
				: GetRegistrationInvocationList(registrations.GetType()).Invoke(registrations, null) as Delegate[];
		}

		private static MethodInfo _regInvocListMi;

		private static MethodInfo GetRegistrationInvocationList(Type registrationsType)
		{
			return _regInvocListMi = _regInvocListMi ?? registrationsType.GetMethod("GetInvocationList");
		}

		private static FieldInfo _regActivatedFi;

		private static FieldInfo GetRegistrationActivatedField(Type registrationType)
		{
			return
				_regActivatedFi =
					_regActivatedFi ?? registrationType.GetField("Activated", BindingFlags.NonPublic | BindingFlags.Instance);
		}


		private static Assembly LoadAssembly(string assemblyName)
		{
			return AssemblyManager.LoadAssemblySafe(assemblyName);
		}

		#endregion
	}
}

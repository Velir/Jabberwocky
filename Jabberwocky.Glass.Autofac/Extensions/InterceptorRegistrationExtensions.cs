using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class InterceptorRegistrationExtensions
	{
		private const string InterceptorsPropertyName = "Autofac.Extras.DynamicProxy2.RegistrationExtensions.InterceptorsPropertyName";

		public const string InterfaceNamedParameter = "interfaceType";
		public const string GlassModelNamedParameter = "model";

		private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();
		private static readonly IEnumerable<Service> EmptyServices = new Service[0];

		public static IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>
			EnableFallbackClassInterceptors<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>(
			this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration)
			where TConcreteReflectionActivatorData : ConcreteReflectionActivatorData
		{
			if (registration == null)
				throw new ArgumentNullException(nameof(registration));

			registration.ActivatorData.ImplementationType = ProxyGenerator.ProxyBuilder.CreateClassProxyType(registration.ActivatorData.ImplementationType, new Type[0], ProxyGenerationOptions.Default);
			return registration.OnPreparing(e => e.Parameters = new Parameter[]
			{
				new PositionalParameter(0,
					GetInterceptorServices(e.Component, registration.ActivatorData.ImplementationType)
						.Select(s => e.Context.ResolveService(s, GetInterceptorParameters(e.Parameters)))
						.Cast<IInterceptor>()
						.ToArray())
			}.Concat(e.Parameters).ToArray());
		}

		public static IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> CustomEnableClassInterceptors<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registration) where TConcreteReflectionActivatorData : ConcreteReflectionActivatorData
		{
			if (registration == null)
				throw new ArgumentNullException(nameof(registration));

			// Create a new proxy generator per call...
			var proxyGenerator = new ProxyGenerator();

			registration.ActivatorData.ImplementationType = (Type)proxyGenerator.ProxyBuilder.CreateClassProxyType((Type)registration.ActivatorData.ImplementationType, new Type[0], ProxyGenerationOptions.Default);
			registration.OnPreparing((Action<PreparingEventArgs>)(e => e.Parameters = (IEnumerable<Parameter>)Enumerable.ToArray<Parameter>(Enumerable.Concat<Parameter>((IEnumerable<Parameter>)new Parameter[1]
	  {
		(Parameter) new PositionalParameter(0, (object) Enumerable.ToArray<IInterceptor>(Enumerable.Cast<IInterceptor>((IEnumerable) Enumerable.Select<Service, object>(GetInterceptorServices(e.Component, (Type) registration.ActivatorData.ImplementationType), (Func<Service, object>) (s => ResolutionExtensions.ResolveService(e.Context, s))))))
	  }, (IEnumerable<Parameter>)e.Parameters))));
			return registration;
		}

		private static IEnumerable<NamedParameter> GetInterceptorParameters(IEnumerable<Parameter> parameters)
		{
			return parameters.OfType<NamedParameter>().Where(namedParam => namedParam.Name == GlassModelNamedParameter || namedParam.Name == InterfaceNamedParameter).ToArray();
		}

		private static IEnumerable<Service> GetInterceptorServices(IComponentRegistration registration, Type implType)
		{
			if (registration == null)
				throw new ArgumentNullException(nameof(registration));
			if (implType == null)
				throw new ArgumentNullException(nameof(implType));

			IEnumerable<Service> enumerable = EmptyServices;
			object obj;
			if (registration.Metadata.TryGetValue(InterceptorsPropertyName, out obj))
				enumerable = enumerable.Concat((IEnumerable<Service>)obj);

			if (implType.IsClass)
				enumerable =
					enumerable.Concat(
						implType.GetCustomAttributes(typeof(InterceptAttribute), true)
							.Cast<InterceptAttribute>()
							.Select(att => att.InterceptorService))
						.Concat(
							implType.GetInterfaces()
								.SelectMany(i => (IEnumerable<object>)i.GetCustomAttributes(typeof(InterceptAttribute), true))
								.Cast<InterceptAttribute>()
								.Select(att => att.InterceptorService));

			return enumerable.ToArray();
		}
	}
}

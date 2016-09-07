using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Glass.Mapper.Sc.Configuration.Attributes;
using Jabberwocky.Core.Utils.Extensions;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Implementation;

namespace Jabberwocky.Glass.Factory.Interceptors
{
	public class FallbackInterceptor : IInterceptor
	{
		private readonly Type _interfaceType;
		private readonly object _model;

		private readonly IGlassTemplateCacheService _templateCache;
		private readonly IImplementationFactory _implementationFactory;

		public FallbackInterceptor(Type interfaceType, object model, IGlassTemplateCacheService templateCache, IImplementationFactory implementationFactory)
		{
			if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
			if (model == null) throw new ArgumentNullException(nameof(model));
			if (templateCache == null) throw new ArgumentNullException(nameof(templateCache));
			if (implementationFactory == null) throw new ArgumentNullException(nameof(implementationFactory));
			_interfaceType = interfaceType;
			_model = model;
			_templateCache = templateCache;
			_implementationFactory = implementationFactory;
		}

		public void Intercept(IInvocation invocation)
		{
			var glassFactoryTypeAttribute = invocation.TargetType.GetCustomAttribute<GlassFactoryTypeAttribute>();
			if (invocation.MethodInvocationTarget.IsAbstract && glassFactoryTypeAttribute != null)
			{
				// Custom resolution required
				ResolveInvocation(invocation, glassFactoryTypeAttribute);
			}
			else
			{
				// Otherwise proceed as normal... This should really only happen if the method is virtual
				try
				{
					invocation.Proceed();
				}
				catch (NotImplementedException)
				{
					ResolveInvocation(invocation, glassFactoryTypeAttribute);
				}
			}

			// In case of value types...
			var returnType = invocation.MethodInvocationTarget.ReturnType;
			if (invocation.ReturnValue == null && returnType.IsValueType)
			{
				invocation.ReturnValue = Activator.CreateInstance(returnType);
			}
		}

		private void ResolveInvocation(IInvocation invocation, GlassFactoryTypeAttribute glassFactoryTypeAttribute)
		{
			var glassType = glassFactoryTypeAttribute.Type;
			var sitecoreAttribute = glassType.GetCustomAttribute<SitecoreTypeAttribute>();

			// If this is a final fallback type that is not associated with a direct Sitecore template
			if (sitecoreAttribute == null) return;

			var templateId = new Guid(sitecoreAttribute.TemplateId);
			var matchingType = _templateCache.GetFallbackImplementingTypeForTemplate(templateId, _interfaceType);

			// This can happen if we fail to find an eligible base template with a corresponding implementation
			// In this case, don't do anything -> invocation maps to default value
			if (matchingType == null) return;

			var fallbackImpl = _implementationFactory.Create(matchingType, _interfaceType, _model);
			var invocationTarget = invocation.MethodInvocationTarget;
			var targetMethod = invocationTarget.IsGenericMethod
				? invocationTarget.GetGenericMethodDefinition()
				: invocationTarget;

			var fallbackMethod = GetFallbackInterfaceMethod(invocation, _interfaceType, invocationTarget, targetMethod);
			if (fallbackMethod != null)
			{
				invocation.ReturnValue = fallbackMethod.Invoke(fallbackImpl, invocation.Arguments);
			}
		}

		private static MethodInfo GetFallbackInterfaceMethod(IInvocation invocation, Type targetInterfaceType, 
			MethodInfo invocationTarget, MethodInfo targetMethod)
		{
			var interfaceCandidates = new[] { targetInterfaceType }.Concat(targetInterfaceType.GetInterfaces());

			foreach (var interfaceType in interfaceCandidates)
			{
				var map = invocation.TargetType.GetInterfaceMap(interfaceType);
				var index = Array.IndexOf(map.TargetMethods, targetMethod);

				if (index == -1) continue;

				MethodInfo interfaceMethod = map.InterfaceMethods[index];
				interfaceMethod = interfaceMethod.IsGenericMethod
					? interfaceMethod.MakeGenericMethod(invocationTarget.GetGenericArguments())
					: interfaceMethod;

				return interfaceMethod;
			}

			return null;
		}
	}
}

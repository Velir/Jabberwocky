using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Jabberwocky.DependencyInjection.AggregateService.Interceptors
{
	public class ResolvingInterceptor : IInterceptor
	{
		private readonly IServiceProvider _context;

		private readonly Dictionary<MethodInfo, Action<IInvocation>> _invocationMap;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResolvingInterceptor"/> class.
		/// </summary>
		/// <param name="interfaceType">Type of the interface to intercept.</param>
		/// <param name="context">The resolution context.</param>
		public ResolvingInterceptor(Type interfaceType, IServiceProvider context)
		{
			_context = context;
			_invocationMap = SetupInvocationMap(interfaceType);
		}

		/// <summary>
		/// Intercepts a method invocation.
		/// </summary>
		/// <param name="invocation">
		/// The method invocation to intercept.
		/// </param>
		public void Intercept(IInvocation invocation)
		{
			if (invocation == null)
			{
				throw new ArgumentNullException("invocation");
			}

			// Generic methods need to use the open generic method definition.
			var method = invocation.Method.IsGenericMethod ? invocation.Method.GetGenericMethodDefinition() : invocation.Method;
			var invocationHandler = _invocationMap[method];
			invocationHandler(invocation);
		}

		private static PropertyInfo GetProperty(MethodInfo method)
		{
			var takesArg = method.GetParameters().Length == 1;
			var hasReturn = method.ReturnType != typeof(void);

			if (takesArg == hasReturn)
			{
				return null;
			}

			return method
				.DeclaringType
				.GetProperties()
				.Where(prop => prop.GetGetMethod() == method)
				.FirstOrDefault();
		}

		private static void InvalidReturnTypeInvocation(IInvocation invocation)
		{
			throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "The method {0} has invalid return type System.Void", invocation.Method));
		}

		private Dictionary<MethodInfo, Action<IInvocation>> SetupInvocationMap(Type interfaceType)
		{
			var methods = interfaceType.GetMethods();

			var methodMap = new Dictionary<MethodInfo, Action<IInvocation>>(methods.Length);
			foreach (var method in methods)
			{
				var returnType = method.ReturnType;

				if (returnType == typeof(void))
				{
					// Any method with 'void' return type (includes property setters) should throw an exception
					methodMap.Add(method, InvalidReturnTypeInvocation);
					continue;
				}

				if (GetProperty(method) != null)
				{
					// All properties should be resolved at proxy instantiation
					var propertyValue = _context.GetService(returnType);
					methodMap.Add(method, invocation => invocation.ReturnValue = propertyValue);
					continue;
				}
				
				// Methods without parameters
				var methodWithoutParams = this.GetType().GetMethod("MethodWithoutParams", BindingFlags.Instance | BindingFlags.NonPublic);
				var methodWithoutParamsDelegate = (Action<IInvocation>)methodWithoutParams.CreateDelegate(typeof(Action<IInvocation>), this);
				methodMap.Add(method, methodWithoutParamsDelegate);
			}

			return methodMap;
		}
	}
}
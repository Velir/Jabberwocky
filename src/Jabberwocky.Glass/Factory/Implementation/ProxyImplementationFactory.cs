using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Jabberwocky.Glass.Factory.Interceptors;

namespace Jabberwocky.Glass.Factory.Implementation
{
	/// <summary>
	/// A Glass Model implementation factory that supports empty constructors, and constructors with a single 'innerItem' glass item
	/// </summary>
	public class ProxyImplementationFactory : IImplementationFactory
	{
		private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

		private const BindingFlags ConstructorFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		private readonly Func<Type, object, FallbackInterceptor> _interceptorFactory;
		private readonly IServiceProvider _serviceProvider;

		public ProxyImplementationFactory(IServiceProvider provider, Func<Type, object, FallbackInterceptor> interceptorFactory)
		{
			if (provider == null) throw new ArgumentNullException(nameof(provider));
			if (interceptorFactory == null) throw new ArgumentNullException(nameof(interceptorFactory));
			_serviceProvider = provider;
			_interceptorFactory = interceptorFactory;
		}

		public T Create<T, TModel>(Type t, TModel glassModel) where T : class
		{
			return Create(t, typeof(T), glassModel) as T;
		}

		public object Create(Type t, Type asType, object glassModel)
		{
			// Return proxy with inner item
			var constructor = t.GetConstructors(ConstructorFlags).FirstOrDefault(info =>
				info.GetParameters().FirstOrDefault(pi => pi.ParameterType.IsInstanceOfType(glassModel)) != null);
			if (constructor != null)
			{
				var parameters = new List<object>();
				foreach (ParameterInfo info in constructor.GetParameters())
				{
					if (info.ParameterType.IsInstanceOfType(glassModel))
					{
						parameters.Add(glassModel);
					}
					else
					{
						parameters.Add(_serviceProvider.GetService(info.ParameterType));
					}
				}

				return ProxyGenerator.CreateClassProxy(t, parameters.ToArray(), _interceptorFactory(asType, glassModel));
			}

			// Return proxy with default constructor
			return ProxyGenerator.CreateClassProxy(t, _interceptorFactory(asType, glassModel));
		}
	}
}

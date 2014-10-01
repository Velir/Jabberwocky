using System;
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

		public ProxyImplementationFactory(Func<Type, object, FallbackInterceptor> interceptorFactory)
		{
			if (interceptorFactory == null) throw new ArgumentNullException("interceptorFactory");
			_interceptorFactory = interceptorFactory;
		}

		public T Create<T, TModel>(Type t, TModel glassModel) where T : class
		{
			return Create(t, typeof(T), glassModel) as T;
		}

		public object Create(Type t, Type asType, object glassModel)
		{
			// Return proxy with inner item
			if (t.GetConstructors(ConstructorFlags).FirstOrDefault(info => info.GetParameters().FirstOrDefault(pi => pi.ParameterType.IsInstanceOfType(glassModel)) != null) != null)
			{
				return ProxyGenerator.CreateClassProxy(t, new[] { glassModel }, _interceptorFactory(asType, glassModel));
			}

			// Return proxy with default constructor
			return ProxyGenerator.CreateClassProxy(t, _interceptorFactory(asType, glassModel));
		}
	}
}

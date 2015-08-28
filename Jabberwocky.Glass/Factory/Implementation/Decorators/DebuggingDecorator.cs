using System;
using System.Reflection;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Exceptions;

namespace Jabberwocky.Glass.Factory.Implementation.Decorators
{
	public class DebuggingDecorator : IImplementationFactory
	{
		private readonly IImplementationFactory _innerFactory;

		protected virtual bool IsDebuggingEnabled { get; }

		public DebuggingDecorator(IImplementationFactory innerFactory, bool debuggingEnabled)
		{
			if (innerFactory == null) throw new ArgumentNullException(nameof(innerFactory));
			_innerFactory = innerFactory;
			IsDebuggingEnabled = debuggingEnabled;
		}

		public T Create<T, TModel>(Type t, TModel glassModel) where T : class
		{
			return Create(t, typeof(T), glassModel) as T;
		}

		public object Create(Type t, Type asType, object glassModel)
		{
			var typeAttribute = t.GetCustomAttribute<GlassFactoryTypeAttribute>();
			var exactGenericGlassType = typeAttribute.Type;

			// Assert that we are able to construct the Glass Factory type with the given Glass model
			if (!exactGenericGlassType.IsInstanceOfType(glassModel))
			{
				if (!IsDebuggingEnabled)
				{
					return null;
				}

				// Otherwise, throw an exception
				throw new ImplementationMismatchException(exactGenericGlassType, glassModel);
			}

			return _innerFactory.Create(t, asType, glassModel);
		}
	}
}

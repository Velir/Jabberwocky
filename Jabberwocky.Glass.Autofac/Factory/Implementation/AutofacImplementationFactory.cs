using System;
using System.Reflection;
using Autofac;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Implementation;

namespace Jabberwocky.Glass.Autofac.Factory.Implementation
{
	/// <summary>
	/// A Glass Model implementation factory that uses Autofac for proxy construction, supporting arbitrary constructor args
	/// </summary>
	public class AutofacImplementationFactory : IImplementationFactory
	{
		private readonly IComponentContext _autofacContainer;

		public AutofacImplementationFactory(IComponentContext autofacContainer)
		{
			if (autofacContainer == null) throw new ArgumentNullException(nameof(autofacContainer));
			_autofacContainer = autofacContainer;
		}

		public T Create<T, TModel>(Type t, TModel glassModel) where T : class
		{
			return Create(t, typeof(T), glassModel) as T;
		}

		public object Create(Type t, Type asType, object glassModel)
		{
			var typeAttribute = t.GetCustomAttribute<GlassFactoryTypeAttribute>();
			var exactGenericGlassType = typeAttribute.Type;

			var implTarget = _autofacContainer.Resolve(t,
				// This is the constructor param for the 'innerItem' glass model
				new TypedParameter(exactGenericGlassType, glassModel),

				// These are the FallbackInterceptor specific constructor params
				new NamedParameter(Extensions.InterceptorRegistrationExtensions.GlassModelNamedParameter, glassModel),
				new NamedParameter(Extensions.InterceptorRegistrationExtensions.InterfaceNamedParameter, asType));

			return implTarget;	 // Should be proxied automatically with new RegistrationExtensions helpers
		}
	}
}

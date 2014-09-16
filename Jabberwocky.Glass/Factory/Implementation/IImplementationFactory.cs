using System;

namespace Jabberwocky.Glass.Factory.Implementation
{
	public interface IImplementationFactory
	{
		T Create<T, TModel>(Type t, TModel glassModel) where T : class;

		Object Create(Type t, Type asType, object glassModel);
	}
}

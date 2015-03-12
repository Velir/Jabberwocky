using System;

namespace Jabberwocky.Glass.Autofac.Mvc.Models.Factory
{
	public interface IViewModelFactory
	{
		TModel Create<TModel>() where TModel : class;
		object Create(Type model);
	}
}

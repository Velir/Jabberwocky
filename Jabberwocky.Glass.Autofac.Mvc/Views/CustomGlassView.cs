using System.Web.Mvc;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Web.Mvc;
using Jabberwocky.Glass.Autofac.Mvc.Models.Factory;

namespace Jabberwocky.Glass.Autofac.Mvc.Views
{
	public abstract class CustomGlassView<TModel> : GlassView<TModel> where TModel : class
	{
		protected override TModel GetModel()
		{
			// TODO: Maybe cache existence of SitecoreType attribute per Type, so as to avoid 'costly' reflection every time a model is created?
			// If this is a Glass Mapper model (template type), then use the base Glass Mapper GlassView implementation
			if (typeof(TModel).IsDefined(typeof(SitecoreTypeAttribute), true))
			{
				return base.GetModel();
			}

			var factory = DependencyResolver.Current.GetService<IViewModelFactory>();
			return factory == null
				? null
				: factory.Create<TModel>();
		}
	}
}

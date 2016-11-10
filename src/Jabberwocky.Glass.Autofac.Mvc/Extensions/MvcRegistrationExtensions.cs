using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Integration.Mvc;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.ModelCache;
using Jabberwocky.Glass.Autofac.Mvc.Models;
using Jabberwocky.Glass.Autofac.Mvc.Models.Factory;
using Jabberwocky.Glass.Mvc.Services;
using Jabberwocky.Glass.Mvc.Util;
using IViewModelFactory = Jabberwocky.Glass.Mvc.Models.Factory.IViewModelFactory;

namespace Jabberwocky.Glass.Autofac.Mvc.Extensions
{
	public static class MvcRegistrationExtensions
	{

		public static void RegisterGlassMvcServices(this ContainerBuilder builder, params string[] assemblyNames)
		{
			RegisterGlassMvcServices(builder, assemblyNames.Select(Assembly.Load).ToArray());
		}

		public static void RegisterGlassMvcServices(this ContainerBuilder builder, params Assembly[] assemblies)
		{
			// Register a custom HtmlHelper
			builder.RegisterType<CustomSitecoreHelper>().AsSelf();

			// Allows for property injection in Views/Partials (but NOT layouts)
			builder.RegisterSource(new ViewRegistrationSource());

			builder.RegisterType<GlassHtml>().As<IGlassHtml>().PreserveExistingDefaults();

			builder.RegisterType<RenderingContextService>().As<IRenderingContextService>().InstancePerLifetimeScope();
			builder.RegisterType<AutofacViewModelFactory>().As<IViewModelFactory>();
			builder.RegisterType<ModelCacheManager>().As<IModelCacheManager>().SingleInstance();

			builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(GlassViewModel<>)).AsSelf().WithAttributeFilter();

			builder.RegisterFilterProvider();
		}
	}
}

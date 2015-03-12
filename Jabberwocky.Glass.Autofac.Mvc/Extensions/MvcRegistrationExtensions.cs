using System.Linq;
using System.Reflection;
using Autofac;
using Jabberwocky.Glass.Autofac.Mvc.Models;
using Jabberwocky.Glass.Autofac.Mvc.Models.Factory;
using Jabberwocky.Glass.Autofac.Mvc.Services;

namespace Jabberwocky.Glass.Autofac.Mvc.Extensions
{
	public static class MvcRegistrationExtensions
	{

		public static void RegisterGlassMvcServices(this ContainerBuilder builder, params string[] assemblies)
		{
			builder.RegisterType<RenderingContextService>().As<IRenderingContextService>().InstancePerLifetimeScope();
			builder.RegisterType<AutofacViewModelFactory>().As<IViewModelFactory>();

			builder.RegisterAssemblyTypes(assemblies.Select(Assembly.Load).ToArray()).AsClosedTypesOf(typeof (GlassViewModel<>)).AsSelf();
		}
	}
}

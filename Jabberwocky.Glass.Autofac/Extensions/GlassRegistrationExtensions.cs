using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Glass.Mapper.Sc;
using Jabberwocky.Core.Utils.Extensions;
using Jabberwocky.Glass.Autofac.Glass;
using Jabberwocky.Glass.Models;
using Jabberwocky.Glass.Services;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class GlassRegistrationExtensions
	{
		/// <summary>
		/// This is the default database that should be used to target the ISitecoreContext and ISitecoreService implementations
		/// </summary>
		private const string DefaultDatabaseName = "master";

		public static ContainerBuilder RegisterGlassModelsAsInterfacesAndSelf(this ContainerBuilder builder, string[] assemblyNames)
		{
			builder.RegisterType<LazyObjectInterceptor>().AsSelf().ExternallyOwned();

			var assemblies = assemblyNames.Select(Assembly.Load);

			foreach (var type in assemblies.SelectMany(a => a.ExportedTypes).Where(type => typeof (GlassBase).IsAssignableFrom(type)))
			{
				// Register lazy versions of each "DIRECT" (no inheritance) interface
				foreach (
					var interfaceType in
						type.GetInterfaces(false).Where(interfaceType => typeof(IGlassBase).IsAssignableFrom(interfaceType)))
				{
					builder.RegisterType(type).As(interfaceType).ExternallyOwned();

					builder.RegisterType(type)
						.PreserveExistingDefaults()
						.Named(interfaceType.FullName + ":lazy", interfaceType)	// Registering named lazy INTERFACE type
							.EnableInterfaceInterceptors()
						.InterceptedBy(typeof(LazyObjectInterceptor))
						.ExternallyOwned();
				}

				builder.RegisterType(type).As(type).ExternallyOwned();

				builder.RegisterType(type)
					.PreserveExistingDefaults()
					.Named(type.FullName + ":lazy", type)	// Registering named lazy CONCRETE type
						.CustomEnableClassInterceptors()
					.InterceptedBy(typeof(LazyObjectInterceptor))
					.ExternallyOwned();
			}

			return builder;
		}

		public static ContainerBuilder RegisterGlassServices(this ContainerBuilder builder)
		{
			builder.RegisterType<ItemService>().As<IItemService>();
			builder.RegisterType<LinkService>().As<ILinkService>();
			builder.RegisterType<SitecoreContext>().As<ISitecoreContext>().ExternallyOwned();
			builder.Register(c =>
			{
				var context = c.Resolve<ISitecoreContext>();
				return context != null && context.Database != null && context.Database.Name != "core"  // prob need to change this.
					? (ISitecoreService)context
					: new SitecoreService(DefaultDatabaseName);
			}).As<ISitecoreService>().ExternallyOwned();

			return builder;
		}
    }
}

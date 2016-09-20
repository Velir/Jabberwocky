using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Glass.Mapper.Sc;
using Jabberwocky.Autofac.Extensions;
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
		private const string DefaultDatabaseName = "web";
		
		/// <summary>
		/// This is the 'core' database
		/// </summary>
		private const string CoreDatabaseName = "core";

		/// <summary>
		/// Registers concrete Glass Models, including lazy variations
		/// </summary>
		/// <remarks>
		/// Note that this does not register interface-based Glass Models, as those do not require DI
		/// </remarks>
		/// <param name="builder"></param>
		/// <param name="assemblyNames"></param>
		/// <returns></returns>
		public static ContainerBuilder RegisterConcreteGlassModelsAsInterfacesAndSelf(this ContainerBuilder builder, params string[] assemblyNames)
		{
			var assemblies = assemblyNames.Select(Assembly.Load).ToArray();
			return RegisterConcreteGlassModelsAsInterfacesAndSelf(builder, assemblies);
		}

		/// <summary>
		/// Registers concrete Glass Models, including lazy variations
		/// </summary>
		/// <remarks>
		/// Note that this does not register interface-based Glass Models, as those do not require DI
		/// </remarks>
		/// <param name="builder"></param>
		/// <param name="assemblies"></param>
		/// <returns></returns>
		public static ContainerBuilder RegisterConcreteGlassModelsAsInterfacesAndSelf(this ContainerBuilder builder, params Assembly[] assemblies)
		{
			builder.RegisterType<LazyObjectInterceptor>().AsSelf().ExternallyOwned();

			foreach (var type in assemblies.SelectMany(a => a.ExportedTypes).Where(type => typeof(GlassBase).IsAssignableFrom(type)))
			{
				// Register lazy versions of each "DIRECT" (no inheritance) interface
				foreach (
					var interfaceType in
						type.GetInterfaces(false).Where(interfaceType => typeof(IGlassBase).IsAssignableFrom(interfaceType)))
				{
					builder.RegisterType(type).As(interfaceType).ExternallyOwned();

					builder.RegisterType(type)
						.PreserveExistingDefaults()
						.Named(interfaceType.FullName + ":lazy", interfaceType) // Registering named lazy INTERFACE type
							.EnableInterfaceInterceptors()
						.InterceptedBy(typeof(LazyObjectInterceptor))
						.ExternallyOwned();
				}

				builder.RegisterType(type).As(type).ExternallyOwned();

				builder.RegisterType(type)
					.PreserveExistingDefaults()
					.Named(type.FullName + ":lazy", type)   // Registering named lazy CONCRETE type
						.CustomEnableClassInterceptors()
					.InterceptedBy(typeof(LazyObjectInterceptor))
					.ExternallyOwned();
			}

			return builder;
		}

		/// <summary>
		/// Registers the ISitecoreContext and ISitecoreService, as well as some basic services built on top of Glass
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="customServiceResolver">An optional delegate, which allows custom registration logic for the SitecoreService</param>
		/// <returns></returns>
		public static ContainerBuilder RegisterGlassServices(this ContainerBuilder builder, Action<ContainerBuilder> customServiceResolver = null)
		{
			builder.RegisterType<ItemService>().As<IItemService>();
			builder.RegisterType<LinkService>().As<ILinkService>();
			builder.RegisterType<SitecoreContext>().As<ISitecoreContext>().ExternallyOwned();

			if (customServiceResolver == null)
			{
				// This is a default resolver for the ISitecoreService
				// It should work across ASP.NET WebForms, MVC, WebAPI with defaults.
				// It makes the assumption that should the current Sitecore Context be indeterminate (or 'core'), then the master DB is preferred

				builder.Register((c, p) =>
				{
					var overriddenDbParam = p.OfType<TypedParameter>().FirstOrDefault(@param => @param.Type == typeof(string));
					var overriddenDb = overriddenDbParam?.Value as string;

					if (!string.IsNullOrEmpty(overriddenDb))
					{
						return new SitecoreService(overriddenDb);
					}

					var context = c.ResolveWithoutExceptions<ISitecoreContext>();
					return context?.Database != null && context.Database.Name != CoreDatabaseName
						? (ISitecoreService)context
						: new SitecoreService(DefaultDatabaseName);
				}).As<ISitecoreService>().ExternallyOwned();
			}
			else
			{
				customServiceResolver(builder);
			}
			
			return builder;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Factory.Builder;
using Jabberwocky.Glass.Autofac.Factory.Implementation;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Builder;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Caching.Providers;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Implementation.Decorators;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class FactoryRegistrationExtensions
	{
		/// <summary>
		/// Registers the Glass Interface Factory, and all Glass Factory Interfaces and Models
		/// 
		/// </summary>
		/// <param name="builder">The container builder.</param>
		/// <param name="assemblyNames">Assemblies to scan for Glass Interfaces and Models.</param>
		/// <returns>
		/// Container Builder
		/// </returns>
		public static ContainerBuilder RegisterGlassFactory(this ContainerBuilder builder, params string[] assemblyNames)
		{
			return InnerRegisterGlassFactory(builder, new ConfigurationOptions(assemblyNames));
		}

		/// <summary>
		/// Registers the Glass Interface Factory, and all Glass Factory Interfaces and Models
		/// 
		/// </summary>
		/// <param name="builder">The container builder.</param>
		/// <param name="options">Custom configuration options for the Glass Interface Factory</param>
		/// <returns>
		/// Container Builder
		/// </returns>
		public static ContainerBuilder RegisterGlassFactory(this ContainerBuilder builder, IConfigurationOptions options)
		{
			return InnerRegisterGlassFactory(builder, options);
		}

		private static ContainerBuilder InnerRegisterGlassFactory(ContainerBuilder builder, IConfigurationOptions options)
		{
			// If necessary, register Glass SitecoreService
			builder.RegisterType<ISitecoreContext>().As<ISitecoreService>().PreserveExistingDefaults().ExternallyOwned();

			builder.RegisterType<FallbackInterceptor>();

			builder.RegisterType<DefaultGlassTypeLoader>().As<IGlassTypesLoader>();

			builder.RegisterType<AutofacImplementationFactory>().Named<IImplementationFactory>("defaultImplementationFactory");
			builder.RegisterDecorator<IImplementationFactory>(
				(c, provider) => new DebuggingDecorator(provider, options.IsDebugEnabled), "defaultImplementationFactory");

			builder.RegisterType<AutofacGlassFactoryBuilder>().AsSelf();
			builder.Register(c => c.Resolve<AutofacGlassFactoryBuilder>(new TypedParameter(typeof(IConfigurationOptions), options)))
				.As<IGlassFactoryBuilder>();

			builder.Register(c => c.Resolve<IGlassFactoryBuilder>().BuildFactory())
				.As<IGlassInterfaceFactory>()
				.SingleInstance();

			builder.Register<Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService>>(c => lookup => new GlassTemplateCacheService(lookup))
				.As<Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService>>();
            builder.Register(c => ((GlassInterfaceFactory)c.Resolve<IGlassInterfaceFactory>()).TemplateCacheService)
				.As<IGlassTemplateCacheService>()
				.SingleInstance();

			return builder;
		}
	}
}

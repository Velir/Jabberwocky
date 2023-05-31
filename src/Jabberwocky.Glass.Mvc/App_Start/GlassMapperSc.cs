#region GlassMapperSc generated code
/*************************************

DO NOT CHANGE THIS FILE - UPDATE GlassMapperScCustom.cs

**************************************/

using GM = Glass.Mapper;
using Glass.Mapper.Configuration;
using Glass.Mapper.Maps;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.IoC;
using Glass.Mapper.Sc.Pipelines.GetChromeData;
using Sitecore.Pipelines;
using System.Linq;

namespace Jabberwocky.Glass.Mvc.App_Start
{
	public class GlassMapperSc : GM.Sc.Pipelines.Initialize.GlassMapperSc
	{
        public override IDependencyResolver CreateResolver()
        {
            var resolver = GlassMapperScCustom.CreateResolver();
            base.CreateResolver(resolver);
            return resolver;
        }
        
        public override IConfigurationLoader[] GetGlassLoaders(GM.Context context)
        { 


          var loaders1 = GlassMapperScCustom.GlassLoaders();        				
          var loaders2 = base.GetGlassLoaders(context);

          return loaders1.Concat(loaders2).ToArray();
        }

        public override void LoadConfigurationMaps(IDependencyResolver resolver, GM.Context context)
        {
            var dependencyResolver = resolver as DependencyResolver;
            if (dependencyResolver == null)
            {
                return;
            }

            if (dependencyResolver.ConfigurationMapFactory is ConfigurationMapConfigFactory)
            {
                GlassMapperScCustom.AddMaps(dependencyResolver.ConfigurationMapFactory);
            }

            IConfigurationMap configurationMap = new ConfigurationMap(dependencyResolver);
            SitecoreFluentConfigurationLoader configurationLoader = configurationMap.GetConfigurationLoader<SitecoreFluentConfigurationLoader>();
            context.Load(configurationLoader);

            base.LoadConfigurationMaps(resolver, context);
        }

	    public override void PostLoad(IDependencyResolver dependencyResolver)
	    {
			GlassMapperScCustom.PostLoad();
		    base.PostLoad(dependencyResolver);
	    }

	}
}
#endregion
using System;

namespace Jabberwocky.Glass.Autofac.Mvc.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigureDatasourceAttribute : Attribute
    {
        protected internal DatasourceResolution Config;

        public ConfigureDatasourceAttribute(DatasourceResolution config)
        {
            Config = config;
        }
    }

    public enum DatasourceResolution
    {
        AllowNesting,
        DisableNesting
    }
}

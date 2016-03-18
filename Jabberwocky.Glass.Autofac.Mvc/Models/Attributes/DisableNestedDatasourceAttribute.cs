namespace Jabberwocky.Glass.Autofac.Mvc.Models.Attributes
{
    public class DisableNestedDatasourceAttribute : ConfigureDatasourceAttribute
    {
        public DisableNestedDatasourceAttribute() 
            : base(DatasourceResolution.DisableNesting)
        {
        }
    }
}

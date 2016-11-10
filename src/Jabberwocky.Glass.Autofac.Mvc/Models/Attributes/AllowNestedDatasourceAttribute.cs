namespace Jabberwocky.Glass.Autofac.Mvc.Models.Attributes
{
    public class AllowNestedDatasourceAttribute : ConfigureDatasourceAttribute
    {
        public AllowNestedDatasourceAttribute() 
            : base(DatasourceResolution.AllowNesting)
        {
        }
    }
}

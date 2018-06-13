namespace Jabberwocky.Glass.Mvc.Models.Attributes
{
    public class AllowNestedDatasourceAttribute : ConfigureDatasourceAttribute
    {
        public AllowNestedDatasourceAttribute() 
            : base(DatasourceResolution.AllowNesting)
        {
        }
    }
}

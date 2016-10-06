using System.Web.Http;

namespace Jabberwocky.WebApi.Sc.PipelineArgs
{
    public class RegisterWebApiPipelineArgs : Sitecore.Pipelines.PipelineArgs
    {
        public HttpConfiguration GlobalConfiguration { get; set; }
    }
}
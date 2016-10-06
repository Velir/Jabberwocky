using System.Web.Http;
using Jabberwocky.WebApi.Sc.PipelineArgs;

namespace Jabberwocky.WebApi.Sc.Pipelines.RegisterWebApi
{
	public class RegisterDefaultRoute
	{
		public virtual void Process(RegisterWebApiPipelineArgs args)
		{
			args.GlobalConfiguration.Routes.MapHttpRoute("defaultApi", "api/{controller}/{id}",
				new
				{
					id = RouteParameter.Optional
				});
		}
	}
}
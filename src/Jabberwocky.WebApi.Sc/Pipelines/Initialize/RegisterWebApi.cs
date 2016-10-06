using System.Web.Http;
using Jabberwocky.WebApi.Sc.PipelineArgs;
using Sitecore.Pipelines;

namespace Jabberwocky.WebApi.Sc.Pipelines.Initialize
{
	public class RegisterWebApi
	{
		public virtual void Process(Sitecore.Pipelines.PipelineArgs args)
		{
			// Register Web API routes & formatters

			var webApiArgs = new RegisterWebApiPipelineArgs
			{
				GlobalConfiguration = GlobalConfiguration.Configuration
			};

			CorePipeline.Run("registerWebApi", webApiArgs, false);
		}
	}
}

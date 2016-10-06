using Jabberwocky.WebApi.Sc.PipelineArgs;

namespace Jabberwocky.WebApi.Sc.Pipelines.RegisterWebApi
{
	public class EnsureInitialized
	{
		public virtual void Process(RegisterWebApiPipelineArgs args)
		{
			args.GlobalConfiguration.EnsureInitialized();
		}
	}
}
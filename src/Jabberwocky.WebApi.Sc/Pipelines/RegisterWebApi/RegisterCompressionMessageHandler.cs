using Jabberwocky.WebApi.Handlers;
using Jabberwocky.WebApi.Sc.PipelineArgs;

namespace Jabberwocky.WebApi.Sc.Pipelines.RegisterWebApi
{
	public class RegisterCompressionMessageHandler
	{
		public virtual void Process(RegisterWebApiPipelineArgs args)
		{
			// Add gzip/deflate compression
			args.GlobalConfiguration.MessageHandlers.Insert(0, new CompressionHandler());
		}
	}
}
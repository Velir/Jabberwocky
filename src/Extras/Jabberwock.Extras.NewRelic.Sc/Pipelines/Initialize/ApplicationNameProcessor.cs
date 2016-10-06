using Jabberwocky.Extras.NewRelic.Sc.Reference;
using Sitecore.Configuration;
using Sitecore.Pipelines;
using NR = NewRelic;

namespace Jabberwocky.Extras.NewRelic.Sc.Pipelines.Initialize
{
	public class ApplicationNameProcessor
	{
		public void Process(PipelineArgs args)
		{
			string appName = Settings.GetSetting(Constants.NewRelicAppNameSetting);

			if (string.IsNullOrEmpty(appName)) return;

			NR.Api.Agent.NewRelic.SetApplicationName(appName);
		}
	}
}
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
			string appSettingsName = Settings.GetAppSetting(Constants.NewRelicAppNameSetting);
			// If there is an app setting with this name, there is no need for this to run.
			if (!string.IsNullOrEmpty(appSettingsName)) return;

			string appName = Settings.GetSetting(Constants.NewRelicAppNameSetting);

			if (string.IsNullOrEmpty(appName)) return;

			NR.Api.Agent.NewRelic.SetApplicationName(appName);
		}
	}
}
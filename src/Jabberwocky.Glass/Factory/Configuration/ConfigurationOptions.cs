namespace Jabberwocky.Glass.Factory.Configuration
{
	public class ConfigurationOptions : IConfigurationOptions
	{
		public string[] Assemblies { get; set; }
		public bool IsDebugEnabled { get; set; }

		public ConfigurationOptions(params string[] assemblies)
		{
			Assemblies = assemblies;
		}

		public ConfigurationOptions(bool debugEnabled, params string[] assemblies)
		{
			IsDebugEnabled = debugEnabled;
			Assemblies = assemblies;
		}
	}
}

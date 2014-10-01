namespace Jabberwocky.Glass.Factory.Configuration
{
	public class ConfigurationOptions : IConfigurationOptions
	{
		public string[] Assemblies { get; set; }

		public ConfigurationOptions(params string[] assemblies)
		{
			Assemblies = assemblies;
		}
	}
}

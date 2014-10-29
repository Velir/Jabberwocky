namespace Jabberwocky.Glass.Factory.Configuration
{
	public interface IConfigurationOptions
	{
		string[] Assemblies { get; set; }
		bool IsDebugEnabled { get; set; }
	}
}

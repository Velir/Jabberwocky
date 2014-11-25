namespace Jabberwocky.Glass.Autofac.Pipelines.Processors
{
	/// <summary>
	/// Represents a Sitecore Pipeline processor with a single Run method
	/// </summary>
	/// <typeparam name="T">The type of Pipeline Args for this particular processor</typeparam>
	public interface IProcessor<in T>
	{
		void Run(T pipelineArgs);
	}
}

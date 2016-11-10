namespace Jabberwocky.Glass.Autofac.Pipelines.Processors
{
	/// <summary>
	/// An abstract implementation of a Sitecore Pipeline Processor that allows for injection of dependencies
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public abstract class ProcessorBase<T> : IProcessor<T>
	{
		public void Process(T pipelineArgs)
		{
			Run(pipelineArgs);
		}

		protected internal abstract void Run(T pipelineArgs);
	}
}

using System;

namespace Jabberwocky.Glass.Autofac.Pipelines.Processors
{
	/// <summary>
	/// An abstract implementation of a Sitecore Pipeline Processor that allows for injection of dependencies
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	/// <typeparam name="T"></typeparam>
	public abstract class ProcessorBase<T> : ProcessorLifetimeBase, IProcessor<T>
	{
		public void Process(T pipelineArgs)
		{
			if (LifetimeScope == null) throw new InvalidOperationException("LifetimeScope was not initialized correctly.");

			using (LifetimeScope)
			{
				Run(pipelineArgs);
			}
		}

		protected internal abstract void Run(T pipelineArgs);
	}
}

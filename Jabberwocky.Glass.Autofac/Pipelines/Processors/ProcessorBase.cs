using System;
using Autofac;

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
		private readonly ILifetimeScope _lifetimeScope;

		protected ProcessorBase(ILifetimeScope lifetimeScope)
		{
			if (lifetimeScope == null) throw new ArgumentNullException("lifetimeScope");
			_lifetimeScope = lifetimeScope;
		}

		public void Process(T pipelineArgs)
		{
			using (_lifetimeScope)
			{
				Run(pipelineArgs);
			}
		}

		protected internal abstract void Run(T pipelineArgs);
	}
}

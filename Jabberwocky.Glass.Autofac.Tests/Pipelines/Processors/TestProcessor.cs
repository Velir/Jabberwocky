using System;
using Autofac;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Tests.Pipelines.Processors
{
	public class TestProcessor : ProcessorBase<string>
	{
		private readonly object _testDependency;

		public TestProcessor(object testDependency, ILifetimeScope lifetimeScope) 
			: base(lifetimeScope)
		{
			if (testDependency == null) throw new ArgumentNullException("testDependency");
			_testDependency = testDependency;
		}

		protected override void Process(string pipelineArgs)
		{
			// do nothing
		}
	}
}

using System;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Tests.Pipelines.Processors
{
	public class TestProcessor : ProcessorBase<string>
	{
		private readonly object _testDependency;

		public TestProcessor(object testDependency)
		{
			if (testDependency == null) throw new ArgumentNullException(nameof(testDependency));
			_testDependency = testDependency;
		}

		protected internal override void Run(string pipelineArgs)
		{
			// do nothing
		}
	}
}

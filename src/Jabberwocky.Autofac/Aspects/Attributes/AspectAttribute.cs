using System;

namespace Jabberwocky.Autofac.Aspects.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
	public class AspectAttribute : Attribute
	{
	}
}

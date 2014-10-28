using Autofac;
using Jabberwocky.Glass.Autofac.Util;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class AutofacRegistrationExtensions
	{
		public static IContainer RegisterContainer(IContainer container)
		{
			// Enable resolving the root container
			var tempBuilder = new ContainerBuilder();
			tempBuilder.Register(c => container).As<IContainer>().SingleInstance();
			tempBuilder.Update(container);

			// Set the AutofacConfig Service Locator
			AutofacConfig.ServiceLocator = container;

			return container;
		}
	}
}

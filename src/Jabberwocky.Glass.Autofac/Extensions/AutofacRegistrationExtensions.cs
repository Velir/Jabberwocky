using Autofac;
using Jabberwocky.Glass.Autofac.Util;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class AutofacRegistrationExtensions
	{

		/// <summary>
		/// Enables the root container to be resolvable, and available for injection
		/// </summary>
		/// <remarks>
		/// It is imperative that this is called after the container is built, in order for the Glass Interface Factory to work
		/// </remarks>
		/// <param name="container"></param>
		/// <returns></returns>
		public static IContainer RegisterContainer(this IContainer container)
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

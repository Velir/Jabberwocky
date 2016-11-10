using Autofac;
using Autofac.Core;

namespace Jabberwocky.Autofac.Extensions
{
    public static class ResolutionExtensions
    {
        public static bool TryResolveWithoutExceptions<T>(this IComponentContext context, out T instance)
        {
            try
            {
                return context.TryResolve(out instance);
            }
            catch (DependencyResolutionException)
            {
            }

            instance = default(T);

            return false;
        }

        public static T ResolveWithoutExceptions<T>(this IComponentContext context)
        {
            try
            {
                return context.Resolve<T>();
            }
            catch (DependencyResolutionException)
            {
            }

            return default(T);
        }
    }
}

using System.Reflection;

namespace Jabberwocky.Core.Utils.Reflection
{
	public static class AssemblyManager
	{
		public static Assembly LoadAssemblySafe(string assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch
			{
				return null;
			}
		}

		public static bool TryLoadAssembly(string assemblyName, out Assembly assembly)
		{
			assembly = LoadAssemblySafe(assemblyName);

			return assembly != null;
		}
	}
}

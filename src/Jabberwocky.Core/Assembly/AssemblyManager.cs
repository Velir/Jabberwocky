using System;
using System.IO;
using System.Linq;
using System.Web;

namespace Jabberwocky.Core.Assembly
{
	public static class AssemblyManager
	{
		public static string[] GetAssemblyNames(string dllName)
		{
			string directory = HttpContext.Current.Server.MapPath("~/bin");

			var dlls = Directory.GetFiles(directory, dllName, SearchOption.AllDirectories);

			return dlls.Select(dll => dll.Substring(dll.LastIndexOf("\\", StringComparison.Ordinal) + 1).Replace(".dll", string.Empty)).ToArray();
		}

		public static System.Reflection.Assembly[] GetAssemblies(string dllName)
		{
			return GetAssemblyNames(dllName).Select(System.Reflection.Assembly.Load).ToArray();
		}
	}
}

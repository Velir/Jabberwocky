using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using Jabberwocky.Core.Utils.Reflection;

namespace Jabberwocky.DependencyInjection.Scanning
{
	/// <summary>
	/// Uses the current Web Application Host to scan for assemblies
	/// </summary>
	/// <remarks>
	/// This class is thread-safe
	/// </remarks>
	public class WebHostAssemblyScanner : IAssemblyScanningService
	{
		protected const string BinVirtualPath = "~/bin";

		/// <summary>
		/// Finds any matching assemblies in the current application 
		/// using the provided dllPattern
		/// </summary>
		/// <param name="dllPattern">A filename pattern that accepts (* or ?) to search for</param>
		/// <remarks>
		/// * (asterisk) matches zero or more characters in that position.
		/// ? (question mark) matches zero or one character in that position.
		/// </remarks>
		/// <returns>
		/// Any matching assembly file names (minus the path and extension), or an empty array
		/// </returns>
		public string[] FindMatchingAssemblyNames(string dllPattern)
		{
			var assemblies = InternalFindMatchingAssemblies(dllPattern);

			return assemblies
				.Select(assembly => assembly.GetName().Name)
				.ToArray();
		}

		/// <summary>
		/// Finds any matching assemblies in the current application 
		/// using the provided dllPattern
		/// </summary>
		/// <param name="dllPattern">A filename pattern that accepts (* or ?) to search for</param>
		/// <remarks>
		/// * (asterisk) matches zero or more characters in that position.
		/// ? (question mark) matches zero or one character in that position.
		/// </remarks>
		/// <returns>Any matching assemblies, or an empty array</returns>
		public Assembly[] FindMatchingAssemblies(string dllPattern)
		{
			var assemblies = InternalFindMatchingAssemblies(dllPattern);

			return assemblies.ToArray();
		}

		protected virtual IEnumerable<Assembly> InternalFindMatchingAssemblies(string dllPattern)
		{
			string directory = HostingEnvironment.MapPath(BinVirtualPath);

			if (directory == null) return Enumerable.Empty<Assembly>();

			var files = Directory.EnumerateFiles(directory, dllPattern, SearchOption.AllDirectories)
				.Select(GetFileNameFromPath);

			var assemblies = files.Select(LoadAssembly)
				.Where(assembly => assembly != null);

			return assemblies;
		}

		private static string GetFileNameFromPath(string filePath)
		{
			return Path.GetFileNameWithoutExtension(filePath);
		}

		protected virtual Assembly LoadAssembly(string assemblyName)
		{
			return AssemblyManager.LoadAssemblySafe(assemblyName);
		}

	}
}

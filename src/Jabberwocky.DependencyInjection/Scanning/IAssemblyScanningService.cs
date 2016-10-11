using System.Reflection;

namespace Jabberwocky.DependencyInjection.Scanning
{
	public interface IAssemblyScanningService
	{
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
		string[] FindMatchingAssemblyNames(string dllPattern);

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
		Assembly[] FindMatchingAssemblies(string dllPattern);
	}
}

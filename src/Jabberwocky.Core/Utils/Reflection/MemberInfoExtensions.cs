using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jabberwocky.Core.Utils.Reflection
{
	public static class MemberInfoExtensions
	{
		public static IEnumerable<T> GetCustomAttributesSafe<T>(this MemberInfo member, bool inherit = false) where T : Attribute
		{
			try
			{
				return member.GetCustomAttributes<T>();
			}
			catch
			{
				return Enumerable.Empty<T>();
			}
		}

		public static T GetCustomAttributeSafe<T>(this MemberInfo member) where T : Attribute
		{
			try
			{
				return member.GetCustomAttribute<T>();
			}
			catch
			{
				return null;
			}
		}
	}
}

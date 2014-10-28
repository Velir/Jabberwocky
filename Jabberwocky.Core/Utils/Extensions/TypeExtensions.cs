using System;
using System.Collections.Generic;
using System.Linq;

namespace Jabberwocky.Core.Utils.Extensions
{
	public static class TypeExtensions
	{
		public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
		{
			return includeInherited || type.BaseType == null
				? type.GetInterfaces()
				: type.GetInterfaces().Except(type.BaseType.GetInterfaces());
		}
	}
}

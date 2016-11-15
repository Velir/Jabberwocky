using System.Collections.Generic;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory
{
	public interface IGlassInterfaceFactory
	{
		T GetItem<T>(IGlassCore model) where T : class;
		IEnumerable<T> GetItems<T>(IEnumerable<IGlassCore> models) where T : class;
	}
}

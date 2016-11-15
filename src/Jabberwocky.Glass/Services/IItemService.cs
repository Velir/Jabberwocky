using System.Collections.Generic;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Services
{
	public interface IItemService
	{
		IEnumerable<IGlassCore> GetDescendants(IGlassCore item);

		IEnumerable<IGlassCore> GetAncestors(IGlassCore item);

		bool HasPresentation(IGlassCore item);

		bool IsContentItem(IGlassCore item);

		bool IsMediaItem(IGlassCore item);
	}
}

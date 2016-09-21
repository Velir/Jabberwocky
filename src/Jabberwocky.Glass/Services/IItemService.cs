using System.Collections.Generic;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Services
{
	public interface IItemService
	{
		IEnumerable<IGlassBase> GetDescendants(IGlassBase item);

		IEnumerable<IGlassBase> GetAncestors(IGlassBase item);

		bool HasPresentation(IGlassBase item);

		bool IsContentItem(IGlassBase item);

		bool IsMediaItem(IGlassBase item);
	}
}

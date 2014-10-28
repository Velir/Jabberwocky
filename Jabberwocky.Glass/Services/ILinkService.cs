using System.Collections.Generic;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Services
{
	public interface ILinkService
	{
		IEnumerable<IGlassBase> GetReferrers(IGlassBase item);
	}
}

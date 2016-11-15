using System.Collections.Generic;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Services
{
	public interface ILinkService
	{
		IEnumerable<IGlassCore> GetReferrers(IGlassCore item);

		IEnumerable<IGlassCore> GetValidLinkTargets(IGlassCore item);
	}
}

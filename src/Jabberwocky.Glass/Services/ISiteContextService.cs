using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Jabberwocky.Glass.Services
{
	public interface ISiteContextService
	{
		string CurrentSiteName { get; }
		string StartPath { get; }
		Language CurrentLanguage { get; }
		DeviceItem CurrentDevice { get; }
		string LanguageName { get; }
	}
}

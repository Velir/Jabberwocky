using AutoSitecore;
using Ploeh.AutoFixture.NUnit2;

namespace Jabberwocky.Glass.Mvc.Tests.Util
{

    /// <summary>
    /// Provides NUnit with auto-wired test data for Sitecore via AutoFixture/AutoSitecore
    /// </summary>
    public class AutoSitecoreAttribute : AutoDataAttribute
    {
        public AutoSitecoreAttribute()
        {
            Fixture.Customize(new AutoSitecoreCustomization());
        }
    }
}

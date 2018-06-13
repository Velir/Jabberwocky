using System;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Jabberwocky.Glass.Mvc.Util;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Helpers;

namespace Jabberwocky.Glass.Mvc.Extensions
{
	public static class HtmlHelperExtensions
	{
		public static IHtmlString ServerSideInclude(this HtmlHelper helper, string serverPath)
		{
			var filePath = HttpContext.Current.Server.MapPath(serverPath);

			var markup = File.ReadAllText(filePath);
			return new HtmlString(markup);
		}

		public static CustomSitecoreHelper Sitecore(this HtmlHelper htmlHelper)
		{
			Assert.ArgumentNotNull(htmlHelper, "htmlHelper");

			var helperFactory = DependencyResolver.Current.GetService<Func<HtmlHelper, CustomSitecoreHelper>>();

			var threadData = ThreadHelper.GetThreadData<CustomSitecoreHelper>();
			if (threadData != null)
				return threadData;

			var data = helperFactory(htmlHelper);
			ThreadHelper.SetThreadData(data);

			return data;
		}

		public static string CsrfTokenHeaderValue(this HtmlHelper htmlHelper)
		{
			string cookieToken, formToken;
			AntiForgery.GetTokens(null, out cookieToken, out formToken);
			return cookieToken + ":" + formToken;
		}
	}
}

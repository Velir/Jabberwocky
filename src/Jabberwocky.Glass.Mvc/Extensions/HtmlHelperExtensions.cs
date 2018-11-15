using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

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

		public static string CsrfTokenHeaderValue(this HtmlHelper htmlHelper)
		{
			string cookieToken, formToken;
			AntiForgery.GetTokens(null, out cookieToken, out formToken);
			return cookieToken + ":" + formToken;
		}
	}
}

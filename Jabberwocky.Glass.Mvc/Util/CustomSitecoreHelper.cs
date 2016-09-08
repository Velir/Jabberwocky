using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Jabberwocky.Glass.Mvc.Attributes;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Helpers;

namespace Jabberwocky.Glass.Mvc.Util
{
	public class CustomSitecoreHelper : SitecoreHelper
	{
		public CustomSitecoreHelper(HtmlHelper htmlHelper) : base(htmlHelper)
		{
		}

		public override HtmlString FormHandler(string controller, string action)
		{
			if (controller.IsEmptyOrNull())
				controller = GetValueFromCurrentRendering("Controller");
			if (action.IsEmptyOrNull())
				action = GetValueFromCurrentRendering("Controller Action");
			if (controller.IsEmptyOrNull())
				return new HtmlString(string.Empty);

			string str = HtmlHelper.Hidden(ValidateFormHandlerAttribute.FormHandlerControllerHiddenInput, controller).ToString();
			if (!action.IsEmptyOrNull())
				str += HtmlHelper.Hidden(ValidateFormHandlerAttribute.FormHandlerActionHiddenInput, action).ToString();

			return new HtmlString(str);
		}
	}
}

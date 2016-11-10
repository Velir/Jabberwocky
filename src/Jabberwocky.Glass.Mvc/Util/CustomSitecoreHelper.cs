using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Jabberwocky.Glass.Mvc.Attributes;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;

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

		protected override Rendering GetRendering(string renderingType, object parameters, params string[] defaultValues)
		{
			Rendering rendering = new Rendering { RenderingType = renderingType };
			int index = 0;
			while (index < defaultValues.Length - 1)
			{
				rendering[defaultValues[index]] = defaultValues[index + 1];
				index += 2;
			}

			// Setup existing parameters from Rendering object in Sitecore (if we can grab the rendering by path or ID)
			if (rendering.RenderingItemPath != null)
			{
				var renderingItem = rendering.RenderingItem;
				if (renderingItem != null)
				{
					rendering.Caching.Cacheable = renderingItem.Caching.Cacheable;
					rendering.Caching.VaryByData = renderingItem.Caching.VaryByData;
					rendering.Caching.VaryByDevice = renderingItem.Caching.VaryByDevice;
					rendering.Caching.VaryByLogin = renderingItem.Caching.VaryByLogin;
					rendering.Caching.VaryByParameters = renderingItem.Caching.VaryByParm;
					rendering.Caching.VaryByQueryString = renderingItem.Caching.VaryByQueryString;
					rendering.Caching.VaryByUser = renderingItem.Caching.VaryByUser;
				}
			}

			// Override any default/existing parameters with those explicitly passed in
			if (parameters != null)
			{
				TypeHelper.GetProperties(parameters).Each(pair => rendering.Properties[pair.Key] = pair.Value.ValueOrDefault(o => o.ToString()));
			}

			return rendering;
		}

		public virtual HtmlString DynamicPlaceholder(string dynamicKey)
		{
			var currentRenderingId = RenderingContext.Current.Rendering.UniqueId;
			return Placeholder(string.Format("{0}_{1}", dynamicKey, currentRenderingId));
		}
	}
}

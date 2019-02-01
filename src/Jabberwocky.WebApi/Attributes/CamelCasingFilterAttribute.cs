using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using Newtonsoft.Json.Serialization;

namespace Jabberwocky.WebApi.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class CamelCasingFilterAttribute : ActionFilterAttribute
	{
		private JsonMediaTypeFormatter _camelCasingFormatter = new JsonMediaTypeFormatter();

		public CamelCasingFilterAttribute()
		{
			_camelCasingFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			ObjectContent content = actionExecutedContext.Response?.Content as ObjectContent;
			if (content != null)
			{
				if (content.Formatter is JsonMediaTypeFormatter)
				{
					actionExecutedContext.Response.Content = new ObjectContent(content.ObjectType, content.Value, _camelCasingFormatter);
				}
			}
		}
	}
}
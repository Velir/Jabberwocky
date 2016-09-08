using System;
using System.Reflection;
using System.Web.Mvc;

namespace Jabberwocky.Glass.Mvc.Attributes
{
    // This type is forwarded... until the obolete version is deprecated, any changes here should be evaluated against the forwarded type

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class HttpGetOrInvalidHttpPostAttribute : ActionMethodSelectorAttribute
	{
		private static readonly HttpGetAttribute InnerHttpGetAttribute = new HttpGetAttribute();
		private static readonly ValidateFormHandlerAttribute InnerFormHandlerAttribute = new ValidateFormHandlerAttribute();

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			if (InnerHttpGetAttribute.IsValidForRequest(controllerContext, methodInfo))
			{
				return true;
			}

			return !InnerFormHandlerAttribute.IsValidForRequest(controllerContext, methodInfo);
		}
	}
}

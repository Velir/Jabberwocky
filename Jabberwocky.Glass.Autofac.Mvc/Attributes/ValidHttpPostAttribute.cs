using System.Reflection;
using System.Web.Mvc;

namespace Jabberwocky.Glass.Autofac.Mvc.Attributes
{
	public class ValidHttpPostAttribute : ActionMethodSelectorAttribute
	{
		private static readonly HttpPostAttribute InnerHttpPostAttribute = new HttpPostAttribute();
		private static readonly ValidateFormHandlerAttribute InnerFormHandlerAttribute = new ValidateFormHandlerAttribute();

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			return InnerHttpPostAttribute.IsValidForRequest(controllerContext, methodInfo) && InnerFormHandlerAttribute.IsValidForRequest(controllerContext, methodInfo);
		}
	}
}

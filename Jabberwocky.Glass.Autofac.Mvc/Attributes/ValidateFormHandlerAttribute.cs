using System;
using System.Reflection;
using System.Web.Mvc;

namespace Jabberwocky.Glass.Autofac.Mvc.Attributes
{
	public class ValidateFormHandlerAttribute : ActionMethodSelectorAttribute
	{
		protected internal const string FormHandlerControllerHiddenInput = "fhController";
		protected internal const string FormHandlerActionHiddenInput = "fhAction";

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			var controller = controllerContext.HttpContext.Request.Form[FormHandlerControllerHiddenInput];
			var action = controllerContext.HttpContext.Request.Form[FormHandlerActionHiddenInput];
			var currentControllerName = GetControllerContextName(controllerContext);

			return !string.IsNullOrWhiteSpace(controller)
				   && !string.IsNullOrWhiteSpace(action)
						 && controller == currentControllerName
				   && methodInfo.Name == action;
		}

		private string GetControllerContextName(ControllerContext controllerContext)
		{
			var typeName = controllerContext.Controller.GetType().Name;
			int index = typeName.IndexOf("Controller", StringComparison.InvariantCulture);
			if (index > 0)
			{
				return typeName.Substring(0, index);
			}

			return typeName;
		}
	}
}

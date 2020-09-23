using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using Jabberwocky.WebApi.Handlers;

namespace Jabberwocky.WebApi.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class CompressionAttribute : ActionFilterAttribute
	{
		private static readonly HashSet<string> SupportedEncodings = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "gzip", "deflate" };
		public override void OnActionExecuted(HttpActionExecutedContext actionContext)
		{
            if (actionContext?.Response?.Content != null)
            {
                var acceptEncodings = actionContext.Response.RequestMessage.Headers.AcceptEncoding;

                if (acceptEncodings != null && acceptEncodings.Any())
                {
                    var encodingType =
                        acceptEncodings.Where(p => SupportedEncodings.Contains(p.Value))
                            .OrderByDescending(p => p.Quality)
                            .FirstOrDefault();

                    string encodingValue = encodingType?.Value;

                    if (encodingValue != null)
                    {
                        actionContext.Response.Content =
                            new CompressedContent(actionContext.Response.Content, encodingValue);
                    }
                }
            }
        }
	}
}
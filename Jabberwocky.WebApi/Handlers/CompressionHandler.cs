using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jabberwocky.WebApi.Handlers
{
	public class CompressionHandler : DelegatingHandler
	{
		private static readonly HashSet<string> SupportedEncodings = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "gzip", "deflate" };

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

			// Don't compress if content is null
			if (response.Content == null)
				return response;

			var acceptEncodings = response.RequestMessage.Headers.AcceptEncoding;

			if (acceptEncodings != null && acceptEncodings.Any())
			{
				var encodingType =
					acceptEncodings.Where(p => SupportedEncodings.Contains(p.Value))
							.OrderByDescending(p => p.Quality)
							.FirstOrDefault();

				string encodingValue = encodingType == null ? null : encodingType.Value;

				if (encodingValue != null)
				{
					response.Content = new CompressedContent(response.Content, encodingValue);
				}
			}

			return response;
		}
	}
}
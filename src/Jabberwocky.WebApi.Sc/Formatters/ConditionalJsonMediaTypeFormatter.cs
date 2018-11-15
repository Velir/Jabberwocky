using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Sitecore.Configuration;

namespace Jabberwocky.WebApi.Sc.Formatters
{
	/// <summary>
	/// Will return settings for all routes not found in the IgnoredRoutes setting.  Otherwise, return default JsonMediaTypeFormatter
	/// </summary>
	public class ConditionalJsonMediaTypeFormatter : JsonMediaTypeFormatter
	{
		private static readonly string[] IgnoredRoutes = Settings.GetSetting("Jabberwocky.WebApi.Sc.IgnoredRoutes").Split(new []{ '|' }, StringSplitOptions.RemoveEmptyEntries);

		public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
		{
			if (IgnoredRoutes.Any(r => request.RequestUri.AbsolutePath.Contains(r)))
			{
				return new JsonMediaTypeFormatter();
			}

			return base.GetPerRequestFormatterInstance(type, request, mediaType);
		}
	}
}

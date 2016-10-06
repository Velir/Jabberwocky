using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Jabberwocky.WebApi.Sc.PipelineArgs;
using Newtonsoft.Json.Serialization;

namespace Jabberwocky.WebApi.Sc.Pipelines.RegisterWebApi
{
	public class RegisterJsonFormatter
	{
		public virtual void Process(RegisterWebApiPipelineArgs args)
		{
			var jsonFormatter = new JsonMediaTypeFormatter
			{
				SerializerSettings = {ContractResolver = new CamelCasePropertyNamesContractResolver()}
			};
			jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			args.GlobalConfiguration.Formatters.Insert(0, jsonFormatter);
		}
	}
}
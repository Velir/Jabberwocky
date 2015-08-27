using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Jabberwocky.Core.Serialization
{
	public class ContractSerializerProvider : ISerializationProvider
	{
		public string SerializeObject<T>(T obj)
		{
			var serializer = new DataContractSerializer(typeof(T));
			using (var stream = new MemoryStream())
			{
				serializer.WriteObject(stream, obj); // UTF-8
				stream.Seek(0, SeekOrigin.Begin);

				using (var stringReader = new StreamReader(stream))
				{
					return stringReader.ReadToEnd();
				}
			}
		}

		public T DeserializeObject<T>(string content)
		{
			var serializer = new DataContractSerializer(typeof(T));
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				return (T)serializer.ReadObject(stream);
			}
		}
	}
}

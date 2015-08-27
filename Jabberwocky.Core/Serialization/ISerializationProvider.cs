namespace Jabberwocky.Core.Serialization
{
	public interface ISerializationProvider
	{
		string SerializeObject<T>(T obj);

		T DeserializeObject<T>(string content);
	}
}

using Jabberwocky.Core.Serialization;

namespace Jabberwocky.Core.Cryptography.Internal
{
	internal static class DefaultCryptoServiceFactory

	{
		internal static AesHmacCryptoService Create(string secret, string digest, string iv)
		{
			var config = new CryptoConfiguration
			{
				SecretKey = secret,
				DigestKey = digest,
				InitializationVector = iv
			};
			return new AesHmacCryptoService(config, new ContractSerializerProvider());
		}
	}
}

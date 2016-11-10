namespace Jabberwocky.Core.Cryptography
{
	public interface IHmacCryptoService
	{
		string EncryptAndSignMessage(string message);

		string DecryptAndValidateMessage(string message);

		string EncryptAndSignMessage<T>(T message);

		T DecryptAndValidateMessage<T>(string message);
	}
}

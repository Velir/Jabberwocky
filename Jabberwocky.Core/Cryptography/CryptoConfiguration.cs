namespace Jabberwocky.Core.Cryptography
{
	public struct CryptoConfiguration
	{
		public string SecretKey { get; set; }
		public string DigestKey { get; set; }
		public string InitializationVector { get; set; }
	}
}

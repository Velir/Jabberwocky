using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Jabberwocky.Core.Cryptography.Internal;
using Jabberwocky.Core.Serialization;

namespace Jabberwocky.Core.Cryptography
{
	/// <summary>
	/// Encrypts and decrypts signed messages
	/// </summary>
	/// <remarks>
	/// This class is thread-safe
	/// </remarks>
	public class AesHmacCryptoService : IHmacCryptoService
	{
		protected ISerializationProvider SerializationProvider { get; }

		/// <summary>
		/// Symmetric Key size for 256-bit cipher
		/// </summary>
		private const int KeySize = 32;
		/// <summary>
		/// AES cipher block-size
		/// </summary>
		private const int BlockSize = 128;
		/// <summary>
		/// Initialization Vector size
		/// </summary>
		private const int SaltSize = 16;

		private byte[] SymmetricKey => DerivedBytes.SymmetricKey;
		private byte[] HashKey => DerivedBytes.HashKey;
		private byte[] Salt => DerivedBytes.Salt;

		private readonly Lazy<KeySaltPair> _lazyDerivedBytes;
		protected KeySaltPair DerivedBytes => _lazyDerivedBytes.Value;

		public static AesHmacCryptoService Create(string secretKey, string digestKey, string initVector)
		{
			return DefaultCryptoServiceFactory.Create(secretKey, digestKey, initVector);
		}

		public AesHmacCryptoService(CryptoConfiguration config, ISerializationProvider serializationProvider)
		{
			if (serializationProvider == null) throw new ArgumentNullException(nameof(serializationProvider));
			if (!IsCryptoConfigurationValid(config)) throw new ArgumentException("All configuration properties must be valid.", nameof(config));
			SerializationProvider = serializationProvider;

			_lazyDerivedBytes = new Lazy<KeySaltPair>(() => GenerateDerivedBytes(config));
		}

		public virtual string EncryptAndSignMessage(string message)
		{
			if (message == null) return null;

			// Prepend a nonce to the message
			var nonce = GenerateNonce();
			var content = nonce.Concat(Encoding.UTF8.GetBytes(message)).ToArray();

			var encryptedContent = CryptContent(content, algorithm => algorithm.CreateEncryptor());
			var hash = ComputeHash(encryptedContent);
			var signedPayload = $"{Convert.ToBase64String(encryptedContent)}|{Convert.ToBase64String(hash)}";

			return signedPayload;
		}

		public virtual string DecryptAndValidateMessage(string message)
		{
			var pieces = message?.Split('|');
			if (pieces?.Length != 2) return null;

			try
			{
				var encryptedContent = Convert.FromBase64String(pieces[0]);
				var hash = Convert.FromBase64String(pieces[1]);

				// Validate that the signed hashes match before continuing
				if (!hash.SequenceEqual(ComputeHash(encryptedContent))) return null;

				var decryptedContent = CryptContent(encryptedContent, algorithm => algorithm.CreateDecryptor());

				// Remove the nonce from the message (by skipping BlockSize)
				return Encoding.UTF8.GetString(decryptedContent, BlockSize, decryptedContent.Length - BlockSize);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		public virtual string EncryptAndSignMessage<T>(T message)
		{
			if (message == null) return null;

			var content = SerializationProvider.SerializeObject(message);
			return EncryptAndSignMessage(content);
		}

		public virtual T DecryptAndValidateMessage<T>(string message)
		{
			var content = DecryptAndValidateMessage(message);
			return content == null ? default(T) : SerializationProvider.DeserializeObject<T>(content);
		}

		#region Protected/Private Helpers

		/// <summary>
		/// Generates a single block of random data
		/// </summary>
		/// <returns></returns>
		protected virtual byte[] GenerateNonce()
		{
			byte[] bytes = new byte[BlockSize];
			using (var rand = new RNGCryptoServiceProvider())
			{
				rand.GetBytes(bytes);
			}

			return bytes;
		}

		protected virtual byte[] CryptContent(byte[] contentBytes, Func<SymmetricAlgorithm, ICryptoTransform> cryptorFunc)
		{
			using (var aes = new RijndaelManaged())
			{
				aes.BlockSize = BlockSize;
				aes.Mode = CipherMode.CBC;
				aes.Key = SymmetricKey;
				aes.IV = Salt;

				using (var cryptor = cryptorFunc(aes))
				{
					return cryptor.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
				}
			}
		}

		protected virtual byte[] ComputeHash(byte[] content)
		{
			using (var hmac = new HMACSHA256(HashKey))
			{
				return hmac.ComputeHash(content);
			}
		}

		private static KeySaltPair GenerateDerivedBytes(CryptoConfiguration config)
		{
			var saltBytes = new byte[SaltSize];
			var charSizeInBytes = sizeof(char);
			var characterCount = Math.Min(SaltSize / charSizeInBytes, config.InitializationVector.Length);
			Encoding.UTF8.GetBytes(config.InitializationVector, 0, characterCount, saltBytes, 0);

			using (var derivePassword = new Rfc2898DeriveBytes(config.SecretKey, saltBytes))
			{
				using (var deriveDigest = new Rfc2898DeriveBytes(config.DigestKey, saltBytes))
				{
					return new KeySaltPair
					{
						SymmetricKey = derivePassword.GetBytes(KeySize),
						Salt = derivePassword.Salt,
						HashKey = deriveDigest.GetBytes(KeySize)
					};
				}
			}
		}

		private static bool IsCryptoConfigurationValid(CryptoConfiguration config)
		{
			return !string.IsNullOrEmpty(config.DigestKey)
				   && !string.IsNullOrEmpty(config.InitializationVector)
				   && !string.IsNullOrEmpty(config.SecretKey);
		}

		#endregion

		protected struct KeySaltPair
		{
			public byte[] SymmetricKey;
			public byte[] Salt;
			public byte[] HashKey;
		}
	}
}

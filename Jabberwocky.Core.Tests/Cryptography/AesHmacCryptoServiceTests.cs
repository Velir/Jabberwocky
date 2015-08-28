using System;
using System.Text;
using Jabberwocky.Core.Cryptography;
using NUnit.Framework;

namespace Jabberwocky.Core.Tests.Cryptography
{
	[TestFixture]
	public class AesHmacCryptoServiceTests
	{
		private AesHmacCryptoService _sut;

		[SetUp]
		public void Setup()
		{
			var secret = "asdf";
			var digest = "fdsa";
			var iv = "123";

			_sut = AesHmacCryptoService.Create(secret, digest, iv);
		}

		[Test]
		public void EncryptAndSignMessage_NullMessage_ReturnsNull()
		{
			Assert.IsNull(_sut.EncryptAndSignMessage(null));
		}

		[Test]
		public void EncryptAndSignMessage_EmptyMessage_Returns()
		{
			var message = _sut.EncryptAndSignMessage(string.Empty);
			Assert.IsNotNullOrEmpty(message);
		}

		[Test]
		public void EncryptAndSignMessage_WithMessage_CanDecryptMessage()
		{
			var message = "hi";
			var encrypted = _sut.EncryptAndSignMessage(message);
			var decrypted = _sut.DecryptAndValidateMessage(encrypted);
			Assert.AreEqual(message, decrypted);
		}

		[Test]
		public void EncryptAndSignMessage_NullObject_ReturnsNull()
		{
			Assert.IsNull(_sut.EncryptAndSignMessage((object)null));
		}

		[Test]
		public void EncryptAndSignMessage_Object_CanRoundTrip()
		{
			var message = new Message { Text = "hello world" };
			var encryptedMessage = _sut.EncryptAndSignMessage(message);
			var decryptedMessage = _sut.DecryptAndValidateMessage<Message>(encryptedMessage);
			Assert.IsNotNull(decryptedMessage);
			Assert.AreEqual(message.Text, decryptedMessage.Text);
		}

		[Test]
		public void DecryptAndValidateMessage_NullMessage_ReturnsNull()
		{
			Assert.IsNull(_sut.DecryptAndValidateMessage(null));
		}

		[Test]
		public void DecryptAndValidateMessage_InvalidMessageFormat_ReturnsNull()
		{
			Assert.IsNull(_sut.DecryptAndValidateMessage("Bad message"));
		}

		[Test]
		public void DecryptAndValidateMessage_InvalidHashFormat_ReturnsNull()
		{
			Assert.IsNull(_sut.DecryptAndValidateMessage("BadMessage|BadHash"));
		}

		[Test]
		public void DecryptAndValidateMessage_InvalidHash_ReturnsNull()
		{
			var message = Convert.ToBase64String(Encoding.UTF8.GetBytes("hello world"));
			var hash = Convert.ToBase64String(Encoding.UTF8.GetBytes("bad hash"));

			Assert.IsNull(_sut.DecryptAndValidateMessage($"{message}|{hash}"));
		}

		[Serializable]
		private class Message
		{
			public string Text { get; set; }
		}
    }
}

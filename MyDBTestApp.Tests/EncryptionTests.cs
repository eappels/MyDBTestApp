using Xunit;

namespace MyDBTestApp.Tests
{
    public class EncryptionTests
    {

        [Fact]
        public void Encrypt_ShouldReturnAnEcryptedString()
        {
            string tmp = StringEncrypter.Encrypt("EddyAppels", "aaa");
            string expected = StringEncrypter.Decrypt(tmp, "aaa");

            string tmp2 = StringEncrypter.Encrypt("EddyAppels", "bbb");
            string actual = StringEncrypter.Decrypt(tmp2, "bbb");

            Assert.Equal(expected, actual);
        }
    }
}
using Xunit;

namespace MyDBTestApp.Tests
{
    public class EncryptionTests
    {

        [Fact]
        public void Encrypt_ShouldReturnSameDataAfterEncryptionAndDecrytion1()
        {
            string tmp = StringEncrypter.Encrypt("EddyAppels", "aaa");
            string expected = StringEncrypter.Decrypt(tmp, "aaa");

            string tmp2 = StringEncrypter.Encrypt("EddyAppels", "bbb");
            string actual = StringEncrypter.Decrypt(tmp2, "bbb");

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Jan Vandamme", "aaabbb")]
        [InlineData("Jan Vandamme", "cc")]
        public void Encrypt_ShouldReturnSameDataAfterEncryptionAndDecrytion(string a, string b)
        {
            string tmp = StringEncrypter.Encrypt("EddyAppels", "aaa");
            string expected = StringEncrypter.Decrypt(tmp, "aaa");

            string tmp2 = StringEncrypter.Encrypt("EddyAppels", "bbb");
            string actual = StringEncrypter.Decrypt(tmp2, "bbb");

            Assert.Equal(expected, actual);
        }
    }
}
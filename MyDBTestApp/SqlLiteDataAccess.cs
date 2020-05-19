using System.Data.SQLite;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;

namespace MyDBTestApp
{
    public class SqlLiteDataAccess
    {

        private static string encryptionkey = "";

        public static List<PersonModel> LoadAllPeople()
        {
            encryptionkey = HttpUtility.UrlEncode("RWRkeUFwcGVsc0RldmVsb3BwZXI");
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<PersonModel>("Select * from Persons", new DynamicParameters());
                List<PersonModel> newlist = new List<PersonModel>();
                foreach (PersonModel model in output.ToList())
                {
                    Console.WriteLine(model.Id + " " + model.Firstname + " " + model.LastName);
                    PersonModel unencryptedpmodel = new PersonModel() { Id = model.Id, Firstname = Decrypt(model.Firstname, encryptionkey), LastName = Decrypt(model.LastName, encryptionkey) };
                    Console.WriteLine(unencryptedpmodel.Id + " " + unencryptedpmodel.Firstname + " " + unencryptedpmodel.LastName);
                    newlist.Add(unencryptedpmodel);
                }
                return newlist;
            }
        }

        public static void SavePerson(PersonModel person)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                PersonModel encryptedmodel = new PersonModel() { Id = person.Id, Firstname = Encrypt(person.Firstname, encryptionkey), LastName = Encrypt(person.LastName, encryptionkey) };
                conn.Execute("Insert into Persons (FirstName, LastName) values (@FirstName, @LastName)", encryptedmodel);
            }
        }

        public static void DeletePerson(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute($"Delete from Persons Where Id =" + id);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        private const int Keysize = 256;
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            int mod4 = passPhrase.Length % 4;
            if (mod4 > 0)
            {
                passPhrase += new string('=', 4 - mod4);
            }
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
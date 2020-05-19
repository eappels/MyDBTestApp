using System.Data.SQLite;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace MyDBTestApp
{
    public class SqlLiteDataAccess
    {
        public static List<PersonModel> LoadAllPeople()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<PersonModel>("Select * from Persons", new DynamicParameters());
                foreach (PersonModel pm in output)
                {
                    PersonModel p2 = new PersonModel()
                    {
                        Firstname = DecryptString("B8 + iXv5 / 8BUQEbHt8 // fGA ==", pm.Firstname),
                        LastName = DecryptString("B8 + iXv5 / 8BUQEbHt8 // fGA ==", pm.LastName)
                    };
                }
                return output.ToList();
            }
        }

        public static void SavePerson(PersonModel person)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var encryptedpersonfirstname = EncryptString("", person.Firstname);
                var encryptedpersonlastname = EncryptString("", person.LastName);
                person.Firstname = EncryptString("B8 + iXv5 / 8BUQEbHt8 // fGA ==", encryptedpersonfirstname);
                person.LastName = EncryptString("B8 + iXv5 / 8BUQEbHt8 // fGA ==", encryptedpersonlastname);
                conn.Execute("Insert into Persons (FirstName, LastName) values (@FirstName, @LastName)", person);
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

        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
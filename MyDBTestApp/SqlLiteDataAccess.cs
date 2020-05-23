using System.Data.SQLite;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;

namespace MyDBTestApp
{
    public class SqlLiteDataAccess
    {

        public static List<PersonModel> LoadAllPeople()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                List<PersonModel> decryptedList = StringEncrypter.Decrypt(conn.Query<PersonModel>("Select * from Persons", new DynamicParameters()).ToList());
                return decryptedList;
            }
        }

        public static PersonModel LoadPerson(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                PersonModel encryptedPerson = (PersonModel)conn.Query<PersonModel>("Select Firstname, LastName from Persons where id = " + id);
                PersonModel decryptedPerson = new PersonModel();
                decryptedPerson.Id = encryptedPerson.Id;
                decryptedPerson.Firstname = StringEncrypter.Decrypt(encryptedPerson.Firstname);
                decryptedPerson.LastName = StringEncrypter.Decrypt(encryptedPerson.LastName);
                return decryptedPerson;
            }
        }

        public static void SavePerson(PersonModel person)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("Insert into Persons (FirstName, LastName) values (@FirstName, @LastName)", StringEncrypter.Encrypt(person));
            }
        }

        public static void DeletePerson(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute($"Delete from Persons Where Id =" + id);
            }
        }

        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
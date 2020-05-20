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
                var output = conn.Query<PersonModel>("Select * from Persons", new DynamicParameters());
                List<PersonModel> decryptedList = new List<PersonModel>();                
                foreach (PersonModel encryptedPersonModel in output.ToList())
                {
                    PersonModel decryptedPersonModel = new PersonModel();
                    decryptedPersonModel.Id = encryptedPersonModel.Id;
                    decryptedPersonModel.Firstname = StringEncrypter.Decrypt(encryptedPersonModel.Firstname);
                    decryptedPersonModel.LastName = StringEncrypter.Decrypt(encryptedPersonModel.LastName);
                    decryptedList.Add(decryptedPersonModel);
                }
                return decryptedList;
            }
        }

        public static void SavePerson(PersonModel person)
        {
            PersonModel encryptedPersonModel = new PersonModel();
            encryptedPersonModel.Id = person.Id;
            encryptedPersonModel.Firstname = StringEncrypter.Encrypt(person.Firstname);
            encryptedPersonModel.LastName = StringEncrypter.Encrypt(person.LastName);
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("Insert into Persons (FirstName, LastName) values (@FirstName, @LastName)", encryptedPersonModel);
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
    }
}
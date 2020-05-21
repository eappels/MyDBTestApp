using Xunit;

namespace MyDBTestApp.Tests
{
    public class SqlLiteDataAccessTests
    {

        [Fact]
        public void LoadConnectionString_ShouldReturnConnectionString()
        {
            string expected = @"Data Source=.\MyTestDatabase.sqlite;Version=3;";

            string actual = SqlLiteDataAccess.LoadConnectionString();

            Assert.Equal(expected, actual);
        }
    }
}
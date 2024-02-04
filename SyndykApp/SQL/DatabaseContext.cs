using System.Data;
using System.Data.SqlClient;
using Dapper;
using SyndykApp.Model;

namespace SyndykApp.SQL
{
    public class DatabaseContext : IDisposable
    {
        private readonly IDbConnection _dbConnection;

        public DatabaseContext()
        {
            _dbConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SyndykDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;");
        }

        public void InsertAdvertisement(Advertisement ad)
        {
            _dbConnection.Execute("INSERT INTO Advertisements (Title, Link, Price, Description) VALUES (@Title, @Link, @Price, @Description)", ad);
        }

        public Advertisement GetAdvertisement(int id)
        {
            return _dbConnection.QueryFirstOrDefault<Advertisement>("SELECT * FROM Advertisements WHERE ID = @ID", new { ID = id });
        }

        public Advertisement[] GetAdvertisements()
        {
            return _dbConnection.Query<Advertisement>("SELECT * FROM Advertisements").ToArray();
        }

        public void UpdateAdvertisement(Advertisement ad)
        {
            _dbConnection.Execute("UPDATE Advertisements SET Title = @Title, Link = @Link, Price = @Price, Description = @Description WHERE ID = @Id", ad);
        }

        public void Dispose()
        {
            _dbConnection.Dispose();
        }
    }
}


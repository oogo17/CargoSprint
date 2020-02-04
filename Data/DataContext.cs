using System;
using MySql.Data.MySqlClient;


namespace cargoSprint.API.Data
{
    public class DataContext : IDisposable
    {
         public MySqlConnection Connection { get; }

        public DataContext(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public void Dispose() => Connection.Dispose();
        
    }
}
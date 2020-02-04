using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using cargoSprint.API.Data;
using MySql.Data.MySqlClient;

namespace cargoSprint.API.Models
{
    public class ItemsQuery
    {
        
        public DataContext Db { get; }

        public ItemsQuery(DataContext db)
        {
            Db = db;
        }

        public async Task<List<Items>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `name`, `description` FROM `items`";
             var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
             return result;

        }
        public async Task<Items> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `name`, `description` FROM `items` WHERE `Id` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }
             private async Task<List<Items>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<Items>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Items(Db)
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }
    }
}
using System.Data;
using System.Threading.Tasks;
using cargoSprint.API.Data;
using MySql.Data.MySqlClient;

namespace cargoSprint.API.Models

{

   public class Items
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


    internal DataContext Db { get; set; }

    internal Items(DataContext db)
    {
            Db= db;
       
    }
    public Items()
    {
        
    }
    public async Task InsertAsync()
    {
        using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `items` (`name`, `description`) VALUES (@name, @description);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            Id = (int) cmd.LastInsertedId;

    }

     public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `items` SET `Name` = @name, `Description` = @description WHERE `Id` = @id;";
            BindParams(cmd);
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

    public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `items` WHERE `Id` = @id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
        }

         private void BindId(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = Id,
            });
        }
              private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@name",
                DbType = DbType.String,
                Value = Name,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@description",
                DbType = DbType.String,
                Value = Description,
            });
        }
    
    }

}
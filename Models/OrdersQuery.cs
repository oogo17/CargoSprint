using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using cargoSprint.API.Data;
using MySql.Data.MySqlClient;


namespace cargoSprint.API.Models
{
    public class OrdersQuery
    {
        public DataContext Db { get; }

        public OrdersQuery(DataContext db)
        {
            Db = db;
        }

        public async Task<List<Orders>> GetAllAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `orders` JOIN `orders_detail` ON `orders`.id = `orders_detail`.id_order JOIN `items` on `items`.id = `orders_detail`.id_item";

            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result;

        }

        public async Task<List<Orders>> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `orders` JOIN `orders_detail` ON @id = `orders_detail`.id_order JOIN `items` on `items`.id = `orders_detail`.id_item WHERE `orders`.id = @id ";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadOneAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result : null;
        }

        private async Task<List<Orders>> ReadOneAsync(DbDataReader reader)
        {
            var ordersList = new List<Orders>();
            var orderDetails = new List<OrdersDetail>();
             var orders = new Orders(Db);
            using (reader)
            {

                while (await reader.ReadAsync())
                {
                   

                    orders.Id = reader.GetInt32(0);
                    orders.Name = reader.GetString(1);
                    orders.Description = reader.GetString(2);
                    orders.Date = reader.GetDateTime(3);
                    orders.OrdersD = orderDetails;

                    var orderItems = new OrdersDetail()
                    {
                        IdOrder = reader.GetInt32(4),
                        IdItem = reader.GetInt32(5),
                        Quantity = reader.GetInt32(6),
                        Item = new Items()
                        {
                            Id = reader.GetInt32(7),
                            Name = reader.GetString(8),
                            Description = reader.GetString(9),

                        },

                    };
                    orders.OrdersD.Add(orderItems);
                    ordersList.Add(orders);
                }


            }
            return ordersList;
        }

        private async Task<List<Orders>> ReadAllAsync(DbDataReader reader)
        {
             var ordersList = new List<Orders>();

            using (reader)
            {

                while (await reader.ReadAsync())
                {
                    var orders = new Orders(Db);


                    orders.Id = reader.GetInt32(0);
                    orders.Name = reader.GetString(1);
                    orders.Description = reader.GetString(2);
                    orders.Date = reader.GetDateTime(3);
                    orders.OrdersD = new List<OrdersDetail>();

                    var orderItems = new OrdersDetail()
                    {

                        IdOrder = reader.GetInt32(4),
                        IdItem = reader.GetInt32(5),
                        Quantity = reader.GetInt32(6),
                        Item = new Items()
                        {
                            Id = reader.GetInt32(7),
                            Name = reader.GetString(8),
                            Description = reader.GetString(9),

                        },

                    };
                    orders.OrdersD.Add(orderItems);

                    if(ordersList.Exists(x =>x.Id == orders.Id))
                    {
                       var order = ordersList.FirstOrDefault(x => x.Id == orders.Id);
                       order.OrdersD.Add(orderItems);

                    }
                    else{
                        ordersList.Add(orders);
                    }
                    


                    
                }


            }
            return ordersList;

            }
       
        }
    
}
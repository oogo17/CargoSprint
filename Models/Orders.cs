using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using cargoSprint.API.CustomExceptions;
using cargoSprint.API.Data;
using MySql.Data.MySqlClient;

namespace cargoSprint.API.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public List<OrdersDetail> OrdersD { get; set; }



        internal DataContext Db { get; set; }

        internal Orders(DataContext db)
        {
            Db = db;

        }
        public Orders()
        {

        }

        public async Task<int> InsertOrdersAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `orders` (`name`, `description`,`date`) VALUES (@name, @description, @date);";
            BindParamsOrders(cmd);
            await cmd.ExecuteNonQueryAsync();
            return Id = (int)cmd.LastInsertedId;



        }

        public async Task InsertOrderDetailAsync(Orders body)
        {



            for (int i = 0; i < body.OrdersD.Count; i++)
            {
                using var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO `orders_detail` (`id_order`, `id_item`,`Quantity`) VALUES (@id_order, @id_item, @quantity);";
                BindParamsOrdersDetails(cmd, body.OrdersD[i]);
                await cmd.ExecuteNonQueryAsync();

            }


        }


        public async Task UpdateAsync(Orders body, List<OrdersDetail> result)
        {
            

            for (int i = 0; i < result.Count; i++)
            {

                if (body.OrdersD.Exists(x => x.IdItem == result[i].IdItem))
                {
                    //Update
                    var bodyMatch=body.OrdersD.Find(x => x.IdItem == result[i].IdItem);
                    using var cmd_d = Db.Connection.CreateCommand();

                    cmd_d.CommandText = @"UPDATE `orders_detail` SET  `Quantity` = @quantity WHERE `id_order`  = @id AND  `id_item` = @id_item;";
                    BindParamsOrdersDetails(cmd_d, bodyMatch);
                    BindId(cmd_d);
                    await cmd_d.ExecuteNonQueryAsync();
                }
                else
                {
                    //Delete
                    using var cmd = Db.Connection.CreateCommand();
                    cmd.CommandText = @"DELETE FROM `orders_detail` WHERE  `orders_detail`.id_order = @id AND  `id_item` = @id_item;";
                    BindParamsOrdersDetails(cmd, result[i]);
                    BindId(cmd);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
             var itemQuery=new ItemsQuery(Db);

            for (int i = 0; i < body.OrdersD.Count; i++)
            {
                var resultItem = await itemQuery.FindOneAsync(body.OrdersD[i].IdItem);

                if(resultItem == null) {
                    FoundExceptions.NotFoundException(body.OrdersD[i].IdItem);
                    }
                        

                if (!result.Exists(x => x.IdItem == body.OrdersD[i].IdItem))
                {
                    //insert
                    using var cmd_i = Db.Connection.CreateCommand();
                    cmd_i.CommandText = @"INSERT INTO `orders_detail` (`id_order`, `id_item`,`Quantity`) VALUES (@id_order, @id_item, @quantity);";
                    BindParamsOrdersDetails(cmd_i, body.OrdersD[i]);
                    await cmd_i.ExecuteNonQueryAsync();
                }
            }

           


        }

        public async Task UpdateOrderAsync() {
             using var cmd_order = Db.Connection.CreateCommand();
            cmd_order.CommandText = @"UPDATE orders SET name = @name ,description = @description, date = @date   WHERE  id = @id;";
            BindParamsOrders(cmd_order);
            BindId(cmd_order);
            await cmd_order.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(List<Orders> order)
        {
            using var cmd = Db.Connection.CreateCommand();


            cmd.CommandText = @"DELETE FROM `orders_detail` WHERE  `orders_detail`.id_order = @id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();

            cmd.CommandText = @"DELETE FROM `orders` WHERE `Id` = @id;";
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

        private void BindParamsOrders(MySqlCommand cmd)
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
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@date",
                DbType = DbType.DateTime,
                Value = Date,
            });

        }

        private void BindParamsOrdersDetails(MySqlCommand cmd, OrdersDetail item)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_order",
                DbType = DbType.Int32,
                Value = Id,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id_item",
                DbType = DbType.Int32,
                Value = item.IdItem,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@quantity",
                DbType = DbType.Int32,
                Value = item.Quantity,
            });

        }



    }
}
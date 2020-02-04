using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using cargoSprint.API.Data;
using MySql.Data.MySqlClient;

namespace cargoSprint.API.Models
{
    public class CustomOrdersData
    {
        internal DataContext Db { get; }

        internal CustomOrdersData(DataContext db)
        {
            Db = db;
        }
        public CustomOrdersData()
        {
            
        }
    
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Items Item { get; set; }

    
    

    public async Task<List<Orders>> GetOrdersAfterAsync(DateTime date)
    {
         using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `orders` JOIN `orders_detail` ON `orders`.id = `orders_detail`.id_order JOIN `items` on `items`.id = `orders_detail`.id_item
            WHERE `orders`.date > @date";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@date",
                DbType = DbType.Date,
                Value = date.Date,
            });

            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result;
        
    }

public async Task<List<Orders>> GetOrdersBetweenAsync(DateTime startDate,DateTime endDate)
    {
         using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `orders` JOIN `orders_detail` ON `orders`.id = `orders_detail`.id_order JOIN `items` on `items`.id = `orders_detail`.id_item
            WHERE `orders`.date BETWEEN @start_date AND @end_date";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@start_date",
                DbType = DbType.Date,
                Value = startDate.Date,
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@end_date",
                DbType = DbType.Date,
                Value = endDate.Date,
            });

            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result;
        
    }

    public async Task<List<CustomOrdersData>> GetMostFrecuentItemAsync(int num)
    {
         using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @" SELECT * FROM (SELECT `id_item`, SUM(`quantity`) as quantity 
            FROM `orders_detail` GROUP BY `id_item`) AS dt JOIN items ON items.id = dt.id_item ORDER BY quantity DESC";
           //SELECT * FROM () AS `dt`
           //JOIN items ON items.id = dt.id_item ORDER BY quantity DESC

            var result = await ReadFrecuentItemsAsync(await cmd.ExecuteReaderAsync());
            var removeItems =result.Count-num;
            result.RemoveRange(num , removeItems);
            return result;
        
    }
    private async Task<List<CustomOrdersData>> ReadFrecuentItemsAsync(DbDataReader reader)
    {
        var frecuentItems = new List<CustomOrdersData>();

          using (reader)
            {

                while (await reader.ReadAsync())
                {
                    var items = new CustomOrdersData(Db);

                    items.ItemId=reader.GetInt32(0);
                    items.Quantity = reader.GetInt32(1);
                    items.Name = reader.GetString(3);
                    items.Description = reader.GetString(4);

                    frecuentItems.Add(items);

                }

            }

            

        return frecuentItems;

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
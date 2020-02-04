using System.Data;
using cargoSprint.API.Data;
using MySql.Data.MySqlClient;

namespace cargoSprint.API.Models
{
    public class OrdersDetail
    {
        public int IdOrder { get; set; }
        public int IdItem { get; set; }
        public int Quantity { get; set; }

        public Items Item { get; set; }

           internal DataContext Db { get; set; }

        internal OrdersDetail(DataContext db)
        {
            Db = db;

        }
        public OrdersDetail()
        {
            
        }



    }
}
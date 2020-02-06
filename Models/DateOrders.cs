using cargoSprint.API.Data;

namespace cargoSprint.API.Models
{
    public class DateOrders
    {
        public int startDate { get; set; }
        public int endDate { get; set; }

        public int date { get; set; }

 internal DataContext Db { get; set; }


    internal DateOrders(DataContext db)
    {
            Db= db;
       
    }
        public DateOrders()
        {
            
        }
    }
}

using System.Threading.Tasks;
using cargoSprint.API.Data;
using cargoSprint.API.Models;
using Microsoft.AspNetCore.Mvc;
   using System.Net;
    using System.Net.Http;


namespace cargoSprint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController: ControllerBase
    {
        public DataContext Db { get; }

        public OrderController(DataContext db)
        {
            Db = db;
        }
        

        // GET api/order
        [HttpGet]
        public async Task<ActionResult> Get()
        {
              await Db.Connection.OpenAsync();
            var query = new OrdersQuery(Db);
            var result = await query.GetAllAsync();
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }
        
        
        // GET api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
              await Db.Connection.OpenAsync();
            var query = new OrdersQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result[0] is null)
                return new NotFoundResult();
            return new OkObjectResult(result[0]);
        }

        // POST api/order
        [HttpPost]
         public async Task<IActionResult> Post([FromBody]Orders body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            var itemQuery=new ItemsQuery(Db);

            for(int i=0;i< body.OrdersD.Count;i++)
            {
                var itemId=body.OrdersD[i].IdItem;
                var findItemID=itemQuery.FindOneAsync(itemId);
               
                if(findItemID.Result == null)
                {
                     var resp = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        {
            Content = new StringContent(string.Format("No item with ID = {0}", findItemID)),
            ReasonPhrase = "Item ID Not Found"
        };
        throw new HttpResponseException(resp.ReasonPhrase);
         
                }
            }
            

            var orderId = await body.InsertOrdersAsync();
           
            await body.InsertOrderDetailAsync(body);
            
            var orderQuery = new OrdersQuery(Db);
            var result=await orderQuery.FindOneAsync(orderId);

            
            
            return new OkObjectResult(result[0]);
        }

        // PUT api/order/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Orders body)
        {
              await Db.Connection.OpenAsync();
            var query = new OrdersQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();

            await result[0].UpdateAsync(body,result[0].OrdersD);

               var order = await query.FindOneAsync(id);
            if (order[0] is null)
                return new NotFoundResult();

            return new OkObjectResult(order[0]);
            
        }

              // DELETE api/order/5
        [HttpDelete("{id}")]
        public  async Task<IActionResult> Delete(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new OrdersQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result[0].DeleteAsync(result);
            return new OkObjectResult(result[0]);
        }
    }
}
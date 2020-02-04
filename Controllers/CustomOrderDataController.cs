using System;
using System.Threading.Tasks;
using cargoSprint.API.Data;
using cargoSprint.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace cargoSprint.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomOrderDataController : ControllerBase
    {
    

        public DataContext Db { get; }

        public CustomOrderDataController(DataContext db)
        {
            Db = db;
        }
          [HttpGet("{date}")]
          [Route("api/[controller]/GetOrderAfter")]
        public async Task<ActionResult> GetOrderAfter(int date)
        {

         DateTime dt = DateTime.ParseExact(date.ToString(), "yyyyMMdd", null);
              await Db.Connection.OpenAsync();
            var query = new CustomOrdersData(Db);
            var result = await query.GetOrdersAfterAsync(dt);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

     

        [HttpGet("{startDate}/{endDate}")]
         [Route("api/[controller]/GetOrderBetween")]
        public async Task<ActionResult> GetOrderBetween(int startDate,int endDate)
        {
             DateTime dtStart = DateTime.ParseExact(startDate.ToString(), "yyyyMMdd", null);
             DateTime dtEnd = DateTime.ParseExact(endDate.ToString(), "yyyyMMdd", null);
              await Db.Connection.OpenAsync();
            var query = new CustomOrdersData(Db);
            var result = await query.GetOrdersBetweenAsync(dtStart,dtEnd);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        [HttpGet("{num}")]
        [Route("api/[controller]/GetMostFrecuent")]
        public async Task<ActionResult> GetMostFrecuent(int num)
        {
              await Db.Connection.OpenAsync();
            var query = new CustomOrdersData(Db);
            var result = await query.GetMostFrecuentItemAsync(num);;
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }


        

    }
}
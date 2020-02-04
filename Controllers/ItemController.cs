using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cargoSprint.API.Data;
using cargoSprint.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace cargoSprint.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        public DataContext Db { get; }
        public ItemController(DataContext db)
        {
           Db = db; 
        }

        // GET api/item
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            await Db.Connection.OpenAsync();
            var query = new ItemsQuery(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        }

        // GET api/item/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
              await Db.Connection.OpenAsync();
            var query = new ItemsQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // POST api/item
        [HttpPost]
         public async Task<IActionResult> Post([FromBody]Items body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            await body.InsertAsync();
            return new OkObjectResult(body);
        }

        // PUT api/item/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Items body)
        {
              await Db.Connection.OpenAsync();
            var query = new ItemsQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.Name = body.Name;
            result.Description = body.Description;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE api/item/5
        [HttpDelete("{id}")]
        public  async Task<IActionResult> Delete(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new ItemsQuery(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }
    


        
    }
}
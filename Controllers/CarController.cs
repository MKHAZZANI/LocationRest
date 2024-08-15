using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using LocationRest.Models;

namespace LocationRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly IMongoCollection<Car> _cars;

        public CarController(IMongoDatabase database)
        {
            _cars = database.GetCollection<Car>("Car");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> Get()
        {
            try
            {
                var cars = await _cars.Find(_ => true).ToListAsync();
                return Ok(cars);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var filter = Builders<Car>.Filter.Eq(x => x.Id, id);

            var car = await _cars.Find(filter).FirstOrDefaultAsync();

            return car is not null ? Ok(car) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Car car)
        {
            if (ModelState.IsValid)
            {
                car.MongoId = null;
                await _cars.InsertOneAsync(car);
                return CreatedAtAction(nameof(GetById), new { id = car.MongoId }, car);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{_id:int}")]
        public async Task<ActionResult> Update(int _id, Car car)
        {
            if (_id != car.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var filter = Builders<Car>.Filter.Eq(x => x.Id, _id); 

            var userExists = await _cars.Find(filter).FirstOrDefaultAsync();
            if (userExists == null)
            {
                return NotFound("User not found");
            }

            var updateResult = await _cars.ReplaceOneAsync(filter, car);

            if (updateResult.MatchedCount == 0)
            {
                return NotFound("User not found for update.");
            }

            return Ok(car);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var filter = Builders<Car>.Filter.Eq(x => x.Id, id);

            await _cars.DeleteOneAsync(filter);

            return Ok();
        }
    }
}
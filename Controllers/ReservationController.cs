using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using LocationRest.Models;

namespace LocationRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMongoCollection<Reservation> _reservations;

        public ReservationController(IMongoDatabase database)
        {
            _reservations = database.GetCollection<Reservation>("Reservation");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> Get()
        {
            try
            {
                var reservations = await _reservations.Find(_ => true).ToListAsync();
                return Ok(reservations);
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
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);

            var reservation = await _reservations.Find(filter).FirstOrDefaultAsync();

            return reservation is not null ? Ok(reservation) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.MongoId = null;
                await _reservations.InsertOneAsync(reservation);
                return CreatedAtAction(nameof(GetById), new { id = reservation.MongoId }, reservation);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);

            await _reservations.DeleteOneAsync(filter);

            return Ok();
        }

        [HttpPut("{_id:int}")]
        public async Task<ActionResult> Update(int _id, Reservation reservation)
        {
            if (_id != reservation.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, _id); // or use _id field depending on your schema

            var userExists = await _reservations.Find(filter).FirstOrDefaultAsync();
            if (userExists == null)
            {
                return NotFound("User not found");
            }

            var updateResult = await _reservations.ReplaceOneAsync(filter, reservation);

            if (updateResult.MatchedCount == 0)
            {
                return NotFound("User not found for update.");
            }

            return Ok(reservation);
        }
    }
}

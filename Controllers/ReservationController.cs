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
        private readonly IMongoCollection<Car> _cars;
        private readonly IMongoCollection<User> _users;

        public ReservationController(IMongoDatabase database)
        {
            _reservations = database.GetCollection<Reservation>("Reservation");
            _cars = database.GetCollection<Car>("Car");
            _users = database.GetCollection<User>("User");
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

            if (reservation == null)
                return NotFound();

            var carFilter = Builders<Car>.Filter.Eq(x => x.Id, reservation.CarId);
            var car = await _cars.Find(carFilter).FirstOrDefaultAsync();

            var userFilter = Builders<User>.Filter.Eq(x => x.Id, reservation.UserId);
            var user = await _users.Find(userFilter).FirstOrDefaultAsync();

            var reservationDetail = new
            {
                ReservationId = reservation.Id,
                DateDeparture = reservation.DateDeparture,
                DateReturn = reservation.DateReturn,
                TotalPrice = reservation.TotalPrice,
                PaymentStatus = reservation.PaymentStatus,
                Attribute = reservation.Attribute,
                Car = car != null ? new
                {
                    Brand = car.Brand,
                    Model = car.Model,
                    Color = car.Color
                } : null,
                User = user != null ? new
                {
                    FullName = user.FullName,
                    Email = user.Email
                } : null
            };

            return Ok(reservationDetail);
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

            var result = await _reservations.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                return NotFound();
            return Ok();
        }

        [HttpPut("{_id:int}")]
        public async Task<ActionResult> Update(int _id, Reservation reservation)
        {
            if (_id != reservation.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, _id);

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


        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<object>>> GetOneUserReservation(int userId)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.UserId, userId);
            var reservations = await _reservations.Find(filter).ToListAsync();

            if (reservations.Count == 0)
                return NotFound("No reservations found for this user.");


            var reservationDetails = new List<object>();

            foreach (var reservation in reservations)
            {
                var carFilter = Builders<Car>.Filter.Eq(x => x.Id, reservation.CarId);
                var car = await _cars.Find(carFilter).FirstOrDefaultAsync();

                var reservationDetail = new
                {
                    ReservationId = reservation.Id,
                    DateDeparture = reservation.DateDeparture,
                    DateReturn = reservation.DateReturn,
                    TotalPrice = reservation.TotalPrice,
                    PaymentStatus = reservation.PaymentStatus,
                    Attribute = reservation.Attribute,
                    Car = car != null ? new
                    {
                        Brand = car.Brand,
                        Model = car.Model,
                        Color = car.Color
                    } : null
                };

                reservationDetails.Add(reservationDetail);
            }
            
            return Ok(reservationDetails);
        }
    }
}

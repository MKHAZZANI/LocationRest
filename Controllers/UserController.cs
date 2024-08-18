using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using LocationRest.Models;

namespace LocationRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        //this is for geting the reservatios for a specific client
        private readonly IMongoCollection<Reservation> _reservations;

        private readonly IMongoCollection<Car> _cars;

        //public UserController(MongoDBSettings mongoDBSettings)
        public UserController(IMongoDatabase database)
        {
            //_users = mongoDBSettings.Database?.GetCollection<User>("User");
            _users = database.GetCollection<User>("User");
            _reservations = database.GetCollection<Reservation>("Reservation");
            _cars = database.GetCollection<Car>("Car");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            try
            {
                var users = await _users.Find(_ => true).ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);

            var user = await _users.Find(filter).FirstOrDefaultAsync();

            return user is not null ? Ok(user) : NotFound();
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                user.MongoId = null; 
                await _users.InsertOneAsync(user);
                return CreatedAtAction(nameof(GetById), new { id = user.MongoId }, user);
            }
            return BadRequest(ModelState);
        }


        [HttpPut("{_id:int}")]
        public async Task<ActionResult> Update(int _id, User user)
        {
            if (_id != user.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, _id); // or use _id field depending on your schema

            var userExists = await _users.Find(filter).FirstOrDefaultAsync();
            if (userExists == null)
            {
                return NotFound("User not found");
            }

            var updateResult = await _users.ReplaceOneAsync(filter, user);

            if (updateResult.MatchedCount == 0)
            {
                return NotFound("User not found for update.");
            }

            return Ok(user);
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, id);

            await _users.DeleteOneAsync(filter);

            return Ok();
        }


        [HttpGet("{id:int}/reservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetClientReservations(int id)
        {
            // Find the user
            var userFilter = Builders<User>.Filter.Eq(x => x.Id, id);
            var user = await _users.Find(userFilter).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Role != UserRole.Client)
            {
                return BadRequest("User is not a client");
            }

            // Fetch all reservations for this user
            var reservationFilter = Builders<Reservation>.Filter.Eq(x => x.UserId, id);
            var reservations = await _reservations.Find(reservationFilter).ToListAsync();

            if (reservations.Count == 0)
            {
                return Ok(new { Message = "No reservations found for this client." });
            }

            // Create a list to hold reservation details
            var reservationDetails = new List<object>();

            foreach (var reservation in reservations)
            {
                // Fetch car details for each reservation
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

            return Ok(new
            {
                ClientName = user.FullName,
                ClientEmail = user.Email,
                Reservations = reservationDetails
            });
        }

    }
}

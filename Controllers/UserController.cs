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

        //public UserController(MongoDBSettings mongoDBSettings)
        public UserController(IMongoDatabase database)
        {
            //_users = mongoDBSettings.Database?.GetCollection<User>("User");
            _users = database.GetCollection<User>("User");
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

    }
}

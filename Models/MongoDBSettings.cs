using MongoDB.Driver;

namespace LocationRest.Models
{
    public class MongoDBSettings
    {
        /*private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;
        public MongoDBSettings(IConfiguration configuration)
        {
            _configuration = configuration;

            var connectionSetting = _configuration.GetConnectionString("DbConnection");
            var mongoUrl = MongoUrl.Create(connectionSetting);
            var mogoClient = new MongoClient(mongoUrl);
            _database = mogoClient.GetDatabase("carRental");

        }
        public IMongoDatabase? Database => _database;*/
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(ConnectionString);
            return client.GetDatabase(DatabaseName);
        }
    }
}

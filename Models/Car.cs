using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LocationRest.Models
{
    public class Car
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? MongoId { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        [BsonElement("Brand")]
        public string Brand { get; set; }

        [BsonElement("Model")]
        public string Model { get; set; }

        [BsonElement("Engin")]
        public string Engin { get; set; }

        [BsonElement("Price")]
        public decimal Price { get; set; }

        [BsonElement("Status")]
        public List<Status> Status { get; set; }

        [BsonElement("Color")]
        public string Color { get; set; }

        [BsonElement("Insurance")]
        public DateTime Insurance { get; set; }

        [BsonElement("Vignette")]
        public DateTime Vignette { get; set; }

        [BsonElement("TechnicalVisit")]
        public DateTime TechnicalVisit { get; set; }
    }

    public class Status
    {

        [BsonElement("Libelle")]
        public string Libelle { get; set; }

        [BsonElement("Photo")]
        public string Photo { get; set; }

        [BsonElement("Statu")]
        public bool Statu { get; set; }

        [BsonElement("Ids")]
        public int Ids { get; set; }
    }

}
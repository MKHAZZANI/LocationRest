using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LocationRest.Models
{
    [BsonCollection("Reservation")]
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; }

        [BsonElement("Id")]
        public int Id { get; set; }

        [BsonElement("DateDeparture")]
        [Required(ErrorMessage = "The departure date is required.")]
        public DateTime DateDeparture { get; set; }

        [BsonElement("DateReturn")]
        [Required(ErrorMessage = "The return date is required.")]
        public DateTime DateReturn { get; set; }

        [BsonElement("TotalPrice")]
        [Required(ErrorMessage = "The total price is required.")]
        public decimal TotalPrice { get; set; }

        [BsonElement("PaymentStatus")]
        [Required(ErrorMessage = "The payment status is required.")]
        public bool PaymentStatus { get; set; }

        [BsonElement("CarId")]
        [Required(ErrorMessage = "The car ID is required.")]
        public int CarId { get; set; }

        [BsonElement("Attribute")]
        [Required(ErrorMessage = "The attribute is required.")]
        public string Attribute { get; set; }

        [BsonElement("UserId")]
        [Required(ErrorMessage = "The user ID is required.")]
        public int UserId { get; set; }

    }
    public class BsonCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}

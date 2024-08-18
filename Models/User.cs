using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LocationRest.Models
{
    [BsonCollection("User")]  
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? MongoId { get; set; }  

        [BsonElement("Id")]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }  

        [BsonElement("FullName")]
        [BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "The full name is required.")]
        public string FullName { get; set; }

        [BsonElement("Email")]
        [BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [BsonElement("Password")]
        [BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "The password is required.")]
        public string Password { get; set; }

        [BsonElement("Avatar")]
        [BsonRepresentation(BsonType.String)]
        public string? Avatar { get; set; }

        [BsonElement("CIN")]
        [BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "The CIN is required.")]
        [RegularExpression(@"^[A-Z][1-2][0-9][1-9]$", ErrorMessage = "You need to follow the pattern of the CIN.")]
        public string CIN { get; set; }

        [BsonElement("PhotoCin")]
        [BsonRepresentation(BsonType.String)]
        public string? PhotoCin { get; set; }

        [BsonElement("BirthDate")]
        [BsonRepresentation(BsonType.DateTime)]
        [Required(ErrorMessage = "The birth date is required.")]
        public DateTime BirthDate { get; set; }

        [BsonElement("Role")]
        [BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "The role is required.")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "The role must be either 'Admin' or 'User'.")]
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        [BsonRepresentation(BsonType.String)]
        Admin,
        [BsonRepresentation(BsonType.String)]
        Client
    }

    //public class BsonCollectionAttribute : Attribute
    //{
    //    public string CollectionName { get; }

    //    public BsonCollectionAttribute(string collectionName)
    //    {
    //        CollectionName = collectionName;
    //    }
    //}
}

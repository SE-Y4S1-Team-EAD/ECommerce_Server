using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class CreateVendorDto
    {
        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } // Should be the User's ObjectId

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Vendor name must be between 3 and 100 characters.")]
        public string Name { get; set; }
    }
}

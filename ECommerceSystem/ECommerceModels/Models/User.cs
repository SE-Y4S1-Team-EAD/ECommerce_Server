using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("Role")]
        public UserRole Role { get; set; } // Administrator, Vendor, CSR, Customer

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = false; // Account activation status

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // For Vendors: Reference to Vendor Profile
        [BsonElement("VendorProfileId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorProfileId { get; set; }
    }

    public enum UserRole
    {
        Administrator,
        Vendor,
        CSR,
        Customer
    }
}

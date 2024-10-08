using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        [Required]
        public string Username { get; set; }

        [BsonElement("Email")]
        [Required]
        public string Email { get; set; }

        [BsonElement("Nic")]
        [Required]
        public string Nic { get; set; }

        [BsonElement("PasswordHash")]
        [Required]
        public string PasswordHash { get; set; }

        [BsonElement("Role")]
        [Required]
        public UserRole Role { get; set; } // Administrator, Vendor, CSR, Customer

        [BsonElement("IsActive")]
        public bool IsActive { get; set; } = false; // Account activation status

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }

    public enum UserRole
    {
        Administrator,
        Vendor,
        CSR,
        Customer
    }
}

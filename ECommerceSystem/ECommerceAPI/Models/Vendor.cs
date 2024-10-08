using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models
{
    public class Vendor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [BsonElement("UserId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } // References the User who is the Vendor

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Rankings")]
        public List<int> Rankings { get; set; } = new List<int>();

        [BsonElement("AverageRanking")]
        public double AverageRanking { get; set; } = 0.0;

        [BsonElement("Comments")]
        public List<Comment> Comments { get; set; } = new List<Comment>();

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }

    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CommentId { get; set; }

        [BsonElement("CustomerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Note: Comments cannot be modified by others, only by the customer who created them
    }
}

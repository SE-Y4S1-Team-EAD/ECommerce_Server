using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }

        [BsonElement("CustomerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonElement("Products")]
        public List<OrderProduct> Products { get; set; } = new List<OrderProduct>();

        [BsonElement("Status")]
        public OrderStatus Status { get; set; } // Processing, PartiallyDelivered, Delivered, Cancelled

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("UpdatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("CancellationNote")]
        public string CancellationNote { get; set; }

        // Additional fields as needed
    }

    public class OrderProduct
    {
        [BsonElement("ProductId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        [BsonElement("Quantity")]
        public int Quantity { get; set; }

        [BsonElement("VendorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        [BsonElement("DeliveryStatus")]
        public DeliveryStatus DeliveryStatus { get; set; } // Ready, Delivered
    }

    public enum OrderStatus
    {
        Processing,
        PartiallyDelivered,
        Delivered,
        Cancelled
    }

    public enum DeliveryStatus
    {
        Ready,
        Delivered
    }
}

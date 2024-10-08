using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class OrderProductDto
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        public string VendorId { get; set; }

        public string DeliveryStatus { get; set; } // e.g., "Processing", "Ready", "Delivered"
    }
}

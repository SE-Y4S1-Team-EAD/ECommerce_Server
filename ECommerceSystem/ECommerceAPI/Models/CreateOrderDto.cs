using ECommerceAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class CreateOrderDto
    {
        [Required]
        public string CustomerId { get; set; }

        [Required]
        public List<OrderProductDto> Products { get; set; } = new List<OrderProductDto>();

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Status { get; set; } // e.g., "Processing", "Delivered", "Cancelled", "Partially Delivered"

        // Optional fields
        public string CancellationNote { get; set; }
    }
}

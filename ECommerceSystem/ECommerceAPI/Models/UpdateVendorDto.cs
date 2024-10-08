using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class UpdateVendorDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Vendor name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        // Add other updatable fields with validation as needed
    }
}

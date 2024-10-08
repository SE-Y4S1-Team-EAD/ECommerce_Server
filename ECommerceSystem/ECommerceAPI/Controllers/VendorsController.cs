using ECommerceAPI.Models;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IUserService _userService;

        public VendorsController(IVendorService vendorService, IUserService userService)
        {
            _vendorService = vendorService;
            _userService = userService;
        }

        // GET: api/vendors
        //[Authorize(Roles = "Administrator,CSR")]
        [HttpGet]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        // GET: api/vendors/{id}
        //[Authorize(Roles = "Administrator,CSR,Vendor")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendorById(string id)
        {
            // Fetch vendor by ID
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
                return NotFound();

            // Check if the requester is a Vendor and ensure they can only access their own profile
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin"); // Replace "Admin" with the appropriate admin role name
            var isVendor = User.IsInRole("Vendor");

            //if (isVendor && vendor.UserId != currentUserId)
            //    return Forbid();

            // Fetch the associated user details from the UserService or directly from the database
            var user = await _userService.GetUserByIdAsync(vendor.UserId); // Ensure your UserService has this method
            if (user == null)
                return NotFound("Associated user not found.");

            // Create a response object containing both vendor and user data
            var response = new
            {
                Vendor = new
                {
                    vendor.VendorId,
                    vendor.Name,
                    vendor.CreatedAt,
                    vendor.UpdatedAt
                },
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Nic,
                    user.Role,
                    user.IsActive
                }
            };

            return Ok(response);
        }


        // POST: api/vendors/create
        //[Authorize(Roles = "Administrator")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserWithVendor([FromBody] CreateUserWithVendorDto createUserWithVendorDto)
        {
            // Create the User
            var userDetails = createUserWithVendorDto.User;
            await _userService.CreateUserAsync(userDetails);

            // Get the User ID
            var userId = userDetails.Id;

            // Create the Vendor using the User ID
            var vendorDto = createUserWithVendorDto.Vendor;
            vendorDto.UserId = userId; // Assign the created User ID to Vendor DTO

            await _vendorService.CreateVendorAsync(vendorDto);

            // Return the created User and Vendor
            return CreatedAtAction(nameof(GetVendorById), new { id = userId }, new { User = userDetails, Vendor = vendorDto });
        }

        // PUT: api/vendors/{id}
        //[Authorize(Roles = "Administrator,Vendor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorDto updateVendorDto)
        {
            // Fetch the vendor by ID
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
                return NotFound();

            // Fetch the associated user by Vendor's UserId
            var user = await _userService.GetUserByIdAsync(vendor.UserId); // Assuming _userService has a GetUserByIdAsync method
            if (user == null)
                return NotFound("Associated user not found.");

            // Check if the requester is a Vendor and ensure they can only update their own profile
            //var currentUserId = GetCurrentUserId();
            //if (User.IsInRole("Vendor") && vendor.UserId != currentUserId)
            //    return Forbid();

            // Update the User details if provided
            if (updateVendorDto.Email != null)
                user.Email = updateVendorDto.Email;

            if (updateVendorDto.id != null)
                user.Id = updateVendorDto.id;

            if (updateVendorDto.PasswordHash != null)
                user.PasswordHash = updateVendorDto.PasswordHash; // Ensure the password is properly hashed

            if (updateVendorDto.Nic != null)
                user.Nic = updateVendorDto.Nic;

            if (updateVendorDto.Username != null)
                user.Username = updateVendorDto.Username;


            user.IsActive = Convert.ToBoolean(updateVendorDto.IsActive);
            user.UpdatedAt = DateTime.UtcNow; // Assuming User entity has an UpdatedAt field

            // Update the Vendor details
            vendor.Name = updateVendorDto.Name ?? vendor.Name;
            vendor.UpdatedAt = DateTime.UtcNow;

            // Save updates to both User and Vendor
            await _userService.UpdateUserAsync(user); // Assuming _userService has an UpdateUserAsync method
            await _vendorService.UpdateVendorAsync(vendor);

            return NoContent();
        }




        // DELETE: api/vendors/{id}
        //[Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(string id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);
            if (vendor == null)
                return NotFound();

            await _vendorService.DeleteVendorAsync(id);
            return NoContent();
        }

        // POST: api/vendors/{id}/rankings
        //[Authorize(Roles = "Customer,Administrator,CSR")]
        [HttpPost("{id}/rankings")]
        public async Task<IActionResult> AddRanking(string id, [FromBody] AddRankingDto addRankingDto)
        {
            // Only Customers can add rankings
            if (!User.IsInRole("Customer"))
                return Forbid();

            await _vendorService.AddRankingAsync(id, addRankingDto.Ranking);
            return Ok();
        }

        // POST: api/vendors/{id}/comments
        //[Authorize(Roles = "Customer,Administrator,CSR")]
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(string id, [FromBody] AddCommentDto addCommentDto)
        {
            // Only Customers can add comments
            if (!User.IsInRole("Customer"))
                return Forbid();

            var comment = new Comment
            {
                CustomerId = GetCurrentUserId(),
                Text = addCommentDto.Text,
                CreatedAt = System.DateTime.UtcNow
            };

            await _vendorService.AddCommentAsync(id, comment);
            return Ok();
        }

        // Helper method to get current user's ID
        private string GetCurrentUserId()
        {
            return User.FindFirst("id")?.Value;
        }
    }

    // DTOs


    public class CreateUserWithVendorDto
    {
        [Required]
        public UserDetails User { get; set; }

        [Required]
        public CreateVendorDto Vendor { get; set; }
    }

    public class UpdateVendorDto
    {
        public string? id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Nic { get; set; }
        public string? PasswordHash { get; set; } // Make sure passwords are handled securely
        public bool? IsActive { get; set; }
    }

    public class AddRankingDto
    {
        [Range(1, 5, ErrorMessage = "Ranking must be between 1 and 5.")]
        public int Ranking { get; set; }
    }

    public class AddCommentDto
    {
        [Required]
        public string Text { get; set; }
    }
}

namespace ECommerceAPI.Models
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
    }
}

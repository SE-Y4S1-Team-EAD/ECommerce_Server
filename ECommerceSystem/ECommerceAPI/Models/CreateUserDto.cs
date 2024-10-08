namespace ECommerceAPI.Models
{
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
    }
}

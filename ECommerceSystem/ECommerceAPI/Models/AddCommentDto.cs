using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class AddCommentDto
    {
        [Required]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string Text { get; set; }
    }
}

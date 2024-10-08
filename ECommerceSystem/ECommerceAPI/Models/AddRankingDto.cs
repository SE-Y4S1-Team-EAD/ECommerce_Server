using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class AddRankingDto
    {
        [Range(1, 5, ErrorMessage = "Ranking must be between 1 and 5.")]
        public int Ranking { get; set; }
    }
}

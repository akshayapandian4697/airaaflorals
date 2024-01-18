using System.ComponentModel.DataAnnotations;

namespace AiraaFlorals.Models
{
    public class Occasion
    {
        [Key]
        public int OccasionId { get; set; }

        [Required]
        public string? OccasionName { get; set; }

        public ICollection<Bouquets>? Bouquets { get; set; }
    }
}

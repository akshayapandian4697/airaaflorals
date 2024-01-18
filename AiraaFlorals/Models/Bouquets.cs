using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiraaFlorals.Models
{
    public class Bouquets
    {
        [Key]
        public int BouquetId { get; set; }

        [Required]
        public string? Name { get; set; }

        public byte[]? Image { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [Required]
        public int? Stock { get; set; }

        public int? OccasionId { get; set; }

        public Occasion? Occasion { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

    }
}

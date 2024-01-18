using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiraaFlorals.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int? ItemQuantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SubTotal { get; set; }

        public int? CartId { get; set; }

        public Cart? Cart { get; set; }

        public int? BouquetsId { get; set; }

        public Bouquets? Bouquets { get; set; }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiraaFlorals.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPrice { get; set; }

        [Required]
        public Boolean IsCheckout { get; set; } = false;

        public int? CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public Order? Order { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiraaFlorals.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public DateTime? OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OrderTotalPrice { get; set; }

        [Required]
        public string? ShippingAddress { get; set;}

        public int? CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public int? CartId { get; set; }

        public Cart? Cart { get; set; }
    }
}

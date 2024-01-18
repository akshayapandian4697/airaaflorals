using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AiraaFlorals.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string? CustomerName { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        public ICollection<Cart>? Carts { get; set; }

        public ICollection<Order>? Orders { get; set; }

    }
}

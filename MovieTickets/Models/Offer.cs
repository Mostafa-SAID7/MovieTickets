using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public string ImageUrl { get; set; }
        public decimal DiscountPercentage { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Details { get; set; }

        public string BannerImage { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

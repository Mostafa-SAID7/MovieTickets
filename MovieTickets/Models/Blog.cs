using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(5000)]
        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Author { get; set; }
    }
}

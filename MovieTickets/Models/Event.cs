using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public DateTime Date { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

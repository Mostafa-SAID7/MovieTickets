using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        public string Location { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime PostedDate { get; set; } = DateTime.Now;
    }
}

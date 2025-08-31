using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Hall
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Hall Name")]
        public string Name { get; set; }   // Example: Hall 1, VIP Hall

        [Display(Name = "Capacity")]
        public int Capacity { get; set; }  // Total seats in the hall

        // 🔹 Relation to Cinema
        [Required]
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}

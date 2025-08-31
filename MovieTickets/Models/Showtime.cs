using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Show Date & Time")]
        public DateTime ShowDateTime { get; set; }

        // 🔹 Relation to Movie
        [Display(Name = "Movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // 🔹 Relation to Cinema
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        // FK -> Hall
        public int HallId { get; set; }
        public Hall? Hall { get; set; }
        // 🔹 Price per ticket (could be different per cinema)
        [Display(Name = "Ticket Price")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TicketPrice { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}

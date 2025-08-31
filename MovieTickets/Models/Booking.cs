using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        // Prefer booking a specific showtime (recommended)
        public int? ShowtimeId { get; set; }
        public Showtime? Showtime { get; set; }
        // Foreign Keys
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int UserId { get; set; }   // better to use int instead of string
        public User User { get; set; }
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }

}

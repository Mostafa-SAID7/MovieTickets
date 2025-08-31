using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Display(Name = "Seat Number")]
        public string SeatNumber { get; set; }   // e.g. "A10", "B3"

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        // Foreign Key
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        // optional FK -> Seat (if you map tickets to specific seats)
        public int? SeatId { get; set; }
        public Seat? Seat { get; set; }
    }
}

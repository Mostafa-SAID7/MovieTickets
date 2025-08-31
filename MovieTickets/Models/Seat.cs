using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Seat
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Seat Number")]
        public string SeatNumber { get; set; }  // Example: A1, A2, B5

        [Display(Name = "Row")]
        public string Row { get; set; }  // Example: A, B, C

        [Display(Name = "Column")]
        public int Column { get; set; }  // Example: 1, 2, 3

        // 🔹 Relation to Cinema (Seats belong to a cinema hall)
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        // FK -> Hall
        public int HallId { get; set; }
        public Hall? Hall { get; set; }
        // 🔹 Optional: Check if seat is reserved
        [Display(Name = "Is Reserved")]
        public bool IsReserved { get; set; } = false;

        // 🔹 Optional: Link to booking (if seat is booked)
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }
    }
}

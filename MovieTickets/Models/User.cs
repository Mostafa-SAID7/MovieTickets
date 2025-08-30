using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        // Relations
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}

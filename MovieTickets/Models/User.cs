using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string? AvatarUrl { get; set; }

        // Relations
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}

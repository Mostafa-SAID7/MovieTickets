using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }

        [MaxLength(250)]
        public string? Url { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}

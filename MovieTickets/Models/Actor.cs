using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        public string? ProfilePicture { get; set; }

        [MaxLength(500)]
        public string? News { get; set; } = string.Empty;

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }

}

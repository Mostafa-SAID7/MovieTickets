using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
    {
    public class Cinema
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? CinemaLogo { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }

}

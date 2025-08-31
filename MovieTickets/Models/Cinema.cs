using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema name is required")]
        [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        [Display(Name = "Cinema Name")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Logo URL")]
        public string? CinemaLogo { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        // Relations
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
        public ICollection<Hall> Halls { get; set; } = new List<Hall>();

    }
}

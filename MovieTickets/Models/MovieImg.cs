using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class MovieImg
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImgUrl { get; set; } = string.Empty;

        // Foreign Key
        [Required]
        public int MovieId { get; set; }

        // Navigation Property
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
}

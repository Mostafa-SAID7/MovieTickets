using MovieTickets.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTickets.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string? ImgUrl { get; set; }   // Main poster

        [NotMapped]
        public List<IFormFile>? UploadedImages { get; set; }

        [Url]
        public string? TrailerUrl { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public MovieStatus MovieStatus { get; set; }

        // Foreign Keys
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Relations
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public ICollection<MovieImg> MovieImgs { get; set; } = new List<MovieImg>();
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }

}

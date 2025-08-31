using Microsoft.AspNetCore.Mvc.Rendering;
using MovieTickets.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Areas.Admin.ViewModels
{
    public class MovieFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0, 10000)]
        public decimal Price { get; set; }

        // Single poster + multiple additional images
        public IFormFile? PosterFile { get; set; }
        public List<IFormFile>? UploadedImages { get; set; }

        public string? ExistingPosterUrl { get; set; }

        // If editing: list of existing image records (id + url)
        public List<MovieImgDto> ExistingImages { get; set; } = new();

        // IDs requested to delete
        public List<int> ImageIdsToDelete { get; set; } = new();

        [Url]
        public string? TrailerUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

        public MovieStatus MovieStatus { get; set; }

        [Required]
        public int CinemaId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<int> SelectedActorIds { get; set; } = new();

        // For selects in view
        public IEnumerable<SelectListItem>? Cinemas { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Actors { get; set; }

        // Concurrency token
        public byte[]? RowVersion { get; set; }
        public string? RowVersionBase64 { get; set; }

    }
    public class MovieImgDto
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
    }
}

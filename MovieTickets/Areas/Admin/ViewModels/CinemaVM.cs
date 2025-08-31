using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MovieTickets.Areas.Admin.ViewModels
{
    public class CinemaVM
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Cinema Name")]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Upload Logo")]
        public IFormFile? LogoFile { get; set; } // upload field

        public string? CinemaLogo { get; set; } // stored path (used in Edit)

        [Required, MaxLength(250)]
        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}

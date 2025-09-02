using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Areas.Admin.ViewModels
{
    public class ActorFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(200)]
        [Display(Name = "Stage Name / Alias")]
        public string? Name { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Biography")]
        public string? Bio { get; set; }

        [Display(Name = "Profile Picture URL")]
        public string? ProfilePicture { get; set; }

        [MaxLength(500)]
        [Display(Name = "News / Highlights")]
        public string? News { get; set; } = string.Empty;
    }
}

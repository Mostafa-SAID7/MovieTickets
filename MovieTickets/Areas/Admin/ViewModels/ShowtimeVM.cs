using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Areas.Admin.ViewModels
{
    public class ShowtimeVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Show Date & Time")]
        public DateTime ShowDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Movie")]
        public int MovieId { get; set; }
        public IEnumerable<SelectListItem> Movies { get; set; }

        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }
        public IEnumerable<SelectListItem> Cinemas { get; set; }

        [Display(Name = "Ticket Price")]
        public decimal TicketPrice { get; set; }
    }
}

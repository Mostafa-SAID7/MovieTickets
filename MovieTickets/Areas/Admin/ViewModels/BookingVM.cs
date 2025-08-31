using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Areas.Admin.ViewModels
{
    public class BookingVM
    {
        public int Id { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        // 🔹 Movie selection
        [Required]
        [Display(Name = "Movie")]
        public int MovieId { get; set; }
        public IEnumerable<SelectListItem> Movies { get; set; } = new List<SelectListItem>();

        // 🔹 User selection
        [Required]
        [Display(Name = "User")]
        public int UserId { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        // 🔹 Cinema selection
        [Required]
        [Display(Name = "Cinema")]
        public int CinemaId { get; set; }
        public IEnumerable<SelectListItem> Cinemas { get; set; } = new List<SelectListItem>();

        // 🔹 Ticket count
        [Display(Name = "Number of Tickets")]
        [Range(1, 20, ErrorMessage = "Please book between 1 and 20 tickets.")]
        public int TicketCount { get; set; }
    }
}

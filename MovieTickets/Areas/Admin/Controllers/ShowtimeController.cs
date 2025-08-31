using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Areas.Admin.ViewModels;
using MovieTickets.Data;
using MovieTickets.Models;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShowtimeController : Controller
    {
        private readonly MovieDbContext _context;

        public ShowtimeController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Showtime
        public async Task<IActionResult> Index()
        {
            var showtimes = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .ToListAsync();
            return View(showtimes);
        }

        // GET: Create
        public IActionResult Create()
        {
            var vm = new ShowtimeVM
            {
                Movies = _context.Movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList(),
                Cinemas = _context.Cinemas.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
            };
            return View(vm);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShowtimeVM vm)
        {
            if (ModelState.IsValid)
            {
                var showtime = new Showtime
                {
                    ShowDateTime = vm.ShowDateTime,
                    MovieId = vm.MovieId,
                    CinemaId = vm.CinemaId,
                    TicketPrice = vm.TicketPrice
                };
                _context.Add(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.Movies = _context.Movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList();
            vm.Cinemas = _context.Cinemas.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View(vm);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return NotFound();

            var vm = new ShowtimeVM
            {
                Id = showtime.Id,
                ShowDateTime = showtime.ShowDateTime,
                MovieId = showtime.MovieId,
                CinemaId = showtime.CinemaId,
                TicketPrice = showtime.TicketPrice,
                Movies = _context.Movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList(),
                Cinemas = _context.Cinemas.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
            };

            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShowtimeVM vm)
        {
            if (id != vm.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var showtime = await _context.Showtimes.FindAsync(id);
                if (showtime == null) return NotFound();

                showtime.ShowDateTime = vm.ShowDateTime;
                showtime.MovieId = vm.MovieId;
                showtime.CinemaId = vm.CinemaId;
                showtime.TicketPrice = vm.TicketPrice;

                _context.Update(showtime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.Movies = _context.Movies.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title }).ToList();
            vm.Cinemas = _context.Cinemas.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View(vm);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (showtime == null) return NotFound();

            return View(showtime);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (showtime == null) return NotFound();

            return View(showtime);
        }

        // POST: Delete
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime != null)
            {
                _context.Showtimes.Remove(showtime);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

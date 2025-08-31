using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.Models;
using MovieTickets.Areas.Admin.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly MovieDbContext _context;

        public BookingController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Booking
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Movie)
                .Include(b => b.User)
                .Include(b => b.Cinema)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Admin/Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Movie)
                .Include(b => b.User)
                .Include(b => b.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Admin/Booking/Create
        public IActionResult Create()
        {
            var vm = new BookingVM
            {
                Movies = _context.Movies.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Title} ({m.Price:C})"
                }).ToList(),

                Users = _context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FullName
                }).ToList(),

                Cinemas = _context.Cinemas.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            ViewBag.MoviePrices = _context.Movies
                .Select(m => new { m.Id, m.Price })
                .ToDictionary(m => m.Id, m => m.Price);

            return View(vm);
        }

        // POST: Admin/Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingVM vm)
        {
            if (ModelState.IsValid)
            {
                var movie = await _context.Movies.FindAsync(vm.MovieId);
                if (movie == null) return NotFound();

                var booking = new Booking
                {
                    BookingDate = vm.BookingDate,
                    MovieId = vm.MovieId,
                    UserId = vm.UserId,
                    CinemaId = vm.CinemaId,
                    TotalPrice = vm.TicketCount * movie.Price
                };

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.Movies = _context.Movies.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Title} ({m.Price:C})"
            }).ToList();

            vm.Users = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName
            }).ToList();

            vm.Cinemas = _context.Cinemas.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(vm);
        }

        // GET: Admin/Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            var vm = new BookingVM
            {
                Id = booking.Id,
                BookingDate = booking.BookingDate,
                TotalPrice = booking.TotalPrice,
                MovieId = booking.MovieId,
                UserId = booking.UserId,
                CinemaId = booking.CinemaId,
                TicketCount = 1,

                Movies = _context.Movies.Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Title} ({m.Price:C})"
                }).ToList(),

                Users = _context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FullName
                }).ToList(),

                Cinemas = _context.Cinemas.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            ViewBag.MoviePrices = _context.Movies
                .Select(m => new { m.Id, m.Price })
                .ToDictionary(m => m.Id, m => m.Price);

            return View(vm);
        }

        // POST: Admin/Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingVM vm)
        {
            if (id != vm.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null) return NotFound();

                var movie = await _context.Movies.FindAsync(vm.MovieId);
                if (movie == null) return NotFound();

                booking.BookingDate = vm.BookingDate;
                booking.MovieId = vm.MovieId;
                booking.UserId = vm.UserId;
                booking.CinemaId = vm.CinemaId;
                booking.TotalPrice = vm.TicketCount * movie.Price;

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(e => e.Id == vm.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            vm.Movies = _context.Movies.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Title} ({m.Price:C})"
            }).ToList();

            vm.Users = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName
            }).ToList();

            vm.Cinemas = _context.Cinemas.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(vm);
        }

        // GET: Admin/Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Movie)
                .Include(b => b.User)
                .Include(b => b.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Admin/Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

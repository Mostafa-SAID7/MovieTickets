using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Areas.Admin.ViewModels;
using MovieTickets.Data;
using MovieTickets.Helpers;
using MovieTickets.Models;

namespace MovieTickets.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MoviesController(MovieDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------------- Index ----------------
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? cinemaId, int pageNumber = 1)
        {
            const int pageSize = 6;

            var moviesQuery = _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.MovieImgs)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                // safe LIKE (depends on DB collation for case sensitivity)
                moviesQuery = moviesQuery.Where(m => EF.Functions.Like(m.Title, $"%{searchString}%"));
            }

            if (categoryId.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.CategoryId == categoryId.Value);
            }

            if (cinemaId.HasValue)
            {
                moviesQuery = moviesQuery.Where(m => m.CinemaId == cinemaId.Value);
            }

            // Prepare select lists for view (no DB access from view)
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            var cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.Cinemas = new SelectList(cinemas, "Id", "Name", cinemaId);

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentCinema = cinemaId;

            var paginatedMovies = await PaginatedList<Movie>.CreateAsync(
                moviesQuery.OrderByDescending(m => m.StartDate),
                pageNumber,
                pageSize
            );

            // If AJAX request return only partial (the _MovieCards partial)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_MovieCards", paginatedMovies);

            return View(paginatedMovies);
        }

        // ---------------- Details ----------------
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .Include(m => m.MovieImgs)
                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            return View(movie);
        }

        // (Add your other actions here: Create/Edit/Delete if any)
    }
}

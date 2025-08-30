using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.Helpers;
using MovieTickets.Models;

namespace MovieTickets.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieDbContext _context;

        public MoviesController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Movies list with Search + Category Filter + Pagination
        public async Task<IActionResult> Index(string searchString, int? categoryId, int pageNumber = 1)
        {
            int pageSize = 6;

            var query = _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => m.Title.Contains(searchString));
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(m => m.MovieCategories.Any(mc => mc.CategoryId == categoryId));
            }

            var categories = await _context.Categories.ToListAsync();
            ViewData["Categories"] = categories;
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = categoryId;

            return View(await PaginatedList<Movie>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize));
        }

        // GET: Movie Details
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                .Include(m => m.MovieImgs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            return View(movie);
        }
    }
}

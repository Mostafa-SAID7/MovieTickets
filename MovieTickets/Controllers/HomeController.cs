using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;

namespace MovieTickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieDbContext _context;

        public HomeController(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int? categoryId, int page = 1, int pageSize = 6)
        {
            var query = _context.Movies
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .AsQueryable();

            // Filter by search text
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Title.Contains(search));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.MovieCategories.Any(mc => mc.CategoryId == categoryId.Value));
            }

            // Pagination
            var totalMovies = await query.CountAsync();
            var movies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            return View(movies);
        }
    }
}

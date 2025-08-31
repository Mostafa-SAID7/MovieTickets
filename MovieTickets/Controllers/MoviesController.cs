using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Areas.Admin.ViewModels;
using MovieTickets.Data;
using MovieTickets.Models;
using System.IO;

namespace MovieTickets.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly long _maxFileBytes = 5 * 1024 * 1024; // 5 MB

        public MoviesController(MovieDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ---------------- Index ----------------
        public async Task<IActionResult> Index(string searchString, int? categoryId, int pageNumber = 1, int pageSize = 6)
        {
            var query = _context.Movies
                .Include(m => m.Cinema)
                .Include(m => m.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
                query = query.Where(m => m.Title.Contains(searchString));

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(m => m.CategoryId == categoryId);

            var total = await query.CountAsync();
            var movies = await query
                .OrderByDescending(m => m.StartDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            return View(movies);
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

 
        // ---------------- Helpers ----------------
        private async Task PopulateVmSelects(MovieFormViewModel vm)
        {
            vm.Cinemas = await _context.Cinemas.OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == vm.CinemaId))
                .ToListAsync();

            vm.Categories = await _context.Categories.OrderBy(c => c.Name)
                .Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == vm.CategoryId))
                .ToListAsync();
        }

      

          

      

       
    }
}

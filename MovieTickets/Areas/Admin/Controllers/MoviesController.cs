using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MovieDbContext context, IWebHostEnvironment env, ILogger<MoviesController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // Index: filter + pagination
        public async Task<IActionResult> Index(string searchString, int? categoryId, int? cinemaId, int pageNumber = 1, int pageSize = 10)
        {
            var q = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
                q = q.Where(m => m.Title.Contains(searchString));

            if (categoryId.HasValue && categoryId > 0)
                q = q.Where(m => m.CategoryId == categoryId.Value);

            if (cinemaId.HasValue && cinemaId > 0)
                q = q.Where(m => m.CinemaId == cinemaId.Value);

            var total = await q.CountAsync();
            var movies = await q
                .OrderByDescending(m => m.StartDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentCinema = cinemaId;
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            if (TempData.ContainsKey("ModelStateErrors")) ViewBag.ModelStateErrors = TempData["ModelStateErrors"];
            return View(movies);
        }

        // Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieImgs)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // Create GET
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View();
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile posterFile)
        {
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(movie?.CategoryId, movie?.CinemaId);
                TempData["ModelStateErrors"] = SerializeModelState();

                return View(movie);
            }

            try
            {
                if (posterFile != null && posterFile.Length > 0)
                {
                    movie.ImgUrl = await SavePosterAsync(posterFile);
                }

                _context.Movies.Add(movie);
                var saved = await _context.SaveChangesAsync();
                TempData["Success"] = $"Movie created. {saved} DB row(s) affected.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create movie failed");
                ModelState.AddModelError("", "Error saving movie: " + ex.Message);
                return View(movie);
            }
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            await PopulateSelectListsAsync(movie.CategoryId, movie.CinemaId);
            return View(movie);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile posterFile)
        {
            if (id != movie.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["ModelStateErrors"] = SerializeModelState();
                await PopulateSelectListsAsync(movie?.CategoryId, movie?.CinemaId);

                return View(movie);
            }

            var existing = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (existing == null) return NotFound();

            // copy properties explicitly
            existing.Title = movie.Title;
            existing.Description = movie.Description;
            existing.Price = movie.Price;
            existing.StartDate = movie.StartDate;
            existing.EndDate = movie.EndDate;
            existing.CategoryId = movie.CategoryId;
            existing.CinemaId = movie.CinemaId;
            existing.TrailerUrl = movie.TrailerUrl;
            existing.MovieStatus = movie.MovieStatus;

            if (posterFile != null && posterFile.Length > 0)
            {
                // delete old local file
                if (!string.IsNullOrEmpty(existing.ImgUrl) && existing.ImgUrl.StartsWith("/uploads/"))
                {
                    DeleteLocalFile(existing.ImgUrl);
                }

                existing.ImgUrl = await SavePosterAsync(posterFile);
            }

            try
            {
                var saved = await _context.SaveChangesAsync();
                TempData["Success"] = $"Movie updated. {saved} DB row(s) affected.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error editing movie {MovieId}", id);
                if (!await _context.Movies.AnyAsync(e => e.Id == movie.Id))
                    return NotFound();

                ModelState.AddModelError("", "Concurrency error. Please try again.");
                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit failed for movie {MovieId}", id);
                ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                return View(movie);
            }
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                if (!string.IsNullOrEmpty(movie.ImgUrl) && movie.ImgUrl.StartsWith("/uploads/"))
                {
                    DeleteLocalFile(movie.ImgUrl);
                }
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Movie deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // helper: save poster and return public path
        private async Task<string> SavePosterAsync(IFormFile posterFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "movies");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using var fs = new FileStream(filePath, FileMode.Create);
            await posterFile.CopyToAsync(fs);
            return $"/uploads/movies/{fileName}";
        }

        private void DeleteLocalFile(string publicPath)
        {
            try
            {
                var path = Path.Combine(_env.WebRootPath, publicPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            catch
            {
                // ignore delete errors
            }
        }

        // helper: populate select lists
        private async Task PopulateSelectListsAsync(int? selectedCategory = null, int? selectedCinema = null)
        {
            var cats = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            var cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();
            ViewData["CategoryId"] = new SelectList(cats, "Id", "Name", selectedCategory);
            ViewData["CinemaId"] = new SelectList(cinemas, "Id", "Name", selectedCinema);
        }

        // helper: serialize ModelState errors
        private string SerializeModelState()
        {
            var errors = ModelState
                .Where(kv => kv.Value.Errors.Count > 0)
                .Select(kv => new {
                    Key = kv.Key,
                    Errors = kv.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage).ToArray()
                }).ToList();
            return System.Text.Json.JsonSerializer.Serialize(errors);
        }
    }
}

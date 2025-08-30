using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MoviesController(MovieDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Movies
        public async Task<IActionResult> Index(string searchString, int? categoryId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(m => m.Title.Contains(searchString));

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(m => m.CategoryId == categoryId.Value);

            var count = await query.CountAsync();

            var movies = await query
                .OrderByDescending(m => m.StartDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            return View(movies);
        }

        // GET: Admin/Movies/Details/5
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

        // GET: Admin/Movies/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSelectListsAsync();
            return View();
        }

        // POST: Admin/Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile posterFile)
        {
            await PopulateSelectListsAsync(movie.CategoryId, movie.CinemaId);

            if (!ModelState.IsValid)
                return View(movie);

            // handle poster upload
            if (posterFile != null && posterFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "movies");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(fs);
                }

                movie.ImgUrl = $"/uploads/movies/{fileName}";
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            await PopulateSelectListsAsync(movie.CategoryId, movie.CinemaId);
            return View(movie);
        }

        // POST: Admin/Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile posterFile)
        {
            if (id != movie.Id) return NotFound();

            await PopulateSelectListsAsync(movie.CategoryId, movie.CinemaId);

            if (!ModelState.IsValid) return View(movie);

            var existing = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (existing == null) return NotFound();

            // update simple props
            existing.Title = movie.Title;
            existing.Description = movie.Description;
            existing.Price = movie.Price;
            existing.StartDate = movie.StartDate;
            existing.EndDate = movie.EndDate;
            existing.CategoryId = movie.CategoryId;
            existing.CinemaId = movie.CinemaId;
            existing.TrailerUrl = movie.TrailerUrl;
            existing.MovieStatus = movie.MovieStatus;

            // handle poster replacement
            if (posterFile != null && posterFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "movies");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                // delete old file if exists and is local
                if (!string.IsNullOrEmpty(existing.ImgUrl) && existing.ImgUrl.StartsWith("/uploads/"))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, existing.ImgUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldPath))
                    {
                        try { System.IO.File.Delete(oldPath); } catch { /* ignore */ }
                    }
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(posterFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(fs);
                }
                existing.ImgUrl = $"/uploads/movies/{fileName}";
            }

            try
            {
                _context.Update(existing);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Movie updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movies.Any(e => e.Id == movie.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Movies/Delete/5
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

        // POST: Admin/Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                // delete image file if local
                if (!string.IsNullOrEmpty(movie.ImgUrl) && movie.ImgUrl.StartsWith("/uploads/"))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, movie.ImgUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldPath))
                    {
                        try { System.IO.File.Delete(oldPath); } catch { /* ignore */ }
                    }
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Movie deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        // helper to fill dropdowns
        private async Task PopulateSelectListsAsync(int? selectedCategory = null, int? selectedCinema = null)
        {
            var cats = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            var cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync();

            ViewData["CategoryId"] = new SelectList(cats, "Id", "Name", selectedCategory);
            ViewData["CinemaId"] = new SelectList(cinemas, "Id", "Name", selectedCinema);
        }
    }
}

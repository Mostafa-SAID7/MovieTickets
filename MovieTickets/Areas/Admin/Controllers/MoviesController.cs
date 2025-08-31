using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Areas.Admin.ViewModels;
using MovieTickets.Data;
using MovieTickets.Models;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MoviesController> _logger;

        // File upload limits / allowed extensions (tweak as needed)
        private readonly string[] _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly long _maxFileBytes = 5 * 1024 * 1024; // 5 MB

        public MoviesController(MovieDbContext context, IWebHostEnvironment env, ILogger<MoviesController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // Index (kept as you had it)
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
        .Include(m => m.MovieActors)
            .ThenInclude(ma => ma.Actor)
        .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // Create GET
        public async Task<IActionResult> Create()
        {
            var vm = new MovieFormViewModel();
            await PopulateVmSelects(vm);
            return View(vm);
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateVmSelects(vm);
                TempData["ModelStateErrors"] = SerializeModelState();
                return View(vm);
            }

            // Validate files before starting transaction
            if (!ValidateFiles(vm.PosterFile, vm.UploadedImages))
            {
                await PopulateVmSelects(vm);
                TempData["ModelStateErrors"] = SerializeModelState();
                return View(vm);
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var movie = new Movie
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    Price = vm.Price,
                    TrailerUrl = vm.TrailerUrl,
                    StartDate = vm.StartDate,
                    EndDate = vm.EndDate,
                    MovieStatus = vm.MovieStatus,
                    CinemaId = vm.CinemaId,
                    CategoryId = vm.CategoryId
                };

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync(); // ensure movie.Id is populated

                // Poster
                if (vm.PosterFile != null && vm.PosterFile.Length > 0)
                {
                    movie.ImgUrl = await SaveFileAsync(vm.PosterFile, "movies");
                }
                await _context.SaveChangesAsync();
                // Additional images
                if (vm.UploadedImages != null)
                {
                    foreach (var f in vm.UploadedImages)
                    {
                        if (f != null && f.Length > 0)
                        {
                            var url = await SaveFileAsync(f, "movies");
                            _context.MovieImgs.Add(new MovieImg { MovieId = movie.Id, ImgUrl = url });
                        }
                    }
                }

                // MovieActors links
                if (vm.SelectedActorIds != null && vm.SelectedActorIds.Any())
                {
                    foreach (var actorId in vm.SelectedActorIds.Distinct())
                        _context.MovieActors.Add(new MovieActor { MovieId = movie.Id, ActorId = actorId });
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Movie created.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Create movie failed");
                ModelState.AddModelError("", "Error saving movie: " + ex.Message);
                await PopulateVmSelects(vm);
                return View(vm);
            }
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.MovieImgs)
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var vm = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Price = movie.Price,
                ExistingPosterUrl = movie.ImgUrl,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                TrailerUrl = movie.TrailerUrl,
                MovieStatus = movie.MovieStatus,
                CinemaId = movie.CinemaId,
                CategoryId = movie.CategoryId,
                SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList(),
                ExistingImages = movie.MovieImgs.Select(mi => new MovieImgDto { Id = mi.Id, ImgUrl = mi.ImgUrl }).ToList(),
                RowVersion = movie.RowVersion
            };

            await PopulateVmSelects(vm);
            return View(vm);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateVmSelects(vm);
                TempData["ModelStateErrors"] = SerializeModelState();
                return View(vm);
            }

            // Validate files
            if (!ValidateFiles(vm.PosterFile, vm.UploadedImages))
            {
                await PopulateVmSelects(vm);
                TempData["ModelStateErrors"] = SerializeModelState();
                return View(vm);
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var existing = await _context.Movies
                    .Include(m => m.MovieImgs)
                    .Include(m => m.MovieActors)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (existing == null) return NotFound();

                // Set original rowversion for concurrency check (if provided)
                if (!string.IsNullOrEmpty(vm.RowVersionBase64))
                    vm.RowVersion = Convert.FromBase64String(vm.RowVersionBase64);


                // Update scalar properties
                existing.Title = vm.Title;
                existing.Description = vm.Description;
                existing.Price = vm.Price;
                existing.TrailerUrl = vm.TrailerUrl;
                existing.StartDate = vm.StartDate;
                existing.EndDate = vm.EndDate;
                existing.MovieStatus = vm.MovieStatus;
                existing.CinemaId = vm.CinemaId;
                existing.CategoryId = vm.CategoryId;
                

                // Replace poster?
                if (vm.PosterFile != null && vm.PosterFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.ImgUrl) && existing.ImgUrl.StartsWith("/uploads/"))
                    {
                        DeleteLocalFile(existing.ImgUrl);
                    }
                    existing.ImgUrl = await SaveFileAsync(vm.PosterFile, "movies");
                }

                // Delete selected additional images
                if (vm.ImageIdsToDelete != null && vm.ImageIdsToDelete.Any())
                {
                    var toDelete = existing.MovieImgs.Where(mi => vm.ImageIdsToDelete.Contains(mi.Id)).ToList();
                    foreach (var img in toDelete)
                    {
                        if (!string.IsNullOrEmpty(img.ImgUrl) && img.ImgUrl.StartsWith("/uploads/"))
                            DeleteLocalFile(img.ImgUrl);

                        _context.MovieImgs.Remove(img);
                    }
                }

                // Add newly uploaded images
                if (vm.UploadedImages != null && vm.UploadedImages.Any())
                {
                    foreach (var f in vm.UploadedImages)
                    {
                        if (f != null && f.Length > 0)
                        {
                            var url = await SaveFileAsync(f, "movies");
                            _context.MovieImgs.Add(new MovieImg { MovieId = existing.Id, ImgUrl = url });
                        }
                    }
                }

                // Sync MovieActors: remove ones not selected, add new ones
                var existingActorIds = existing.MovieActors.Select(ma => ma.ActorId).ToList();
                var toRemove = existing.MovieActors.Where(ma => !vm.SelectedActorIds.Contains(ma.ActorId)).ToList();
                foreach (var rem in toRemove) _context.MovieActors.Remove(rem);

                var toAdd = vm.SelectedActorIds.Where(aid => !existingActorIds.Contains(aid)).Distinct();
                foreach (var add in toAdd) _context.MovieActors.Add(new MovieActor { MovieId = existing.Id, ActorId = add });

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Movie updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await tx.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency conflict when editing movie {MovieId}", id);
                ModelState.AddModelError("", "This record was modified by another user. Please reload and try again.");
                await PopulateVmSelects(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Edit failed for movie {MovieId}", id);
                ModelState.AddModelError("", "Error saving changes: " + ex.Message);
                await PopulateVmSelects(vm);
                return View(vm);
            }
        }

        // Delete confirmed (keeps your logic)
        // GET: Admin/Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Load the movie with all dependent collections that might block deletion
            var movie = await _context.Movies
                .Include(m => m.MovieImgs)
                .Include(m => m.MovieActors)
                .Include(m => m.MovieCategories)
                .Include(m => m.Bookings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }

            // If there are bookings, do NOT delete - inform the user.
            // (If you prefer to cascade-delete bookings, remove this check and delete them explicitly.)
            if (movie.Bookings != null && movie.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete movie because there are existing bookings for it. Please remove bookings first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete poster file (if local)
                if (!string.IsNullOrEmpty(movie.ImgUrl) && movie.ImgUrl.StartsWith("/uploads/"))
                {
                    try
                    {
                        DeleteLocalFile(movie.ImgUrl);
                    }
                    catch (Exception ex)
                    {
                        // Log but continue — not fatal
                        _logger.LogWarning(ex, "Failed to delete poster file for movie {MovieId}", id);
                    }
                }

                // Delete additional images (files + DB rows)
                if (movie.MovieImgs != null && movie.MovieImgs.Any())
                {
                    foreach (var img in movie.MovieImgs.ToList())
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(img.ImgUrl) && img.ImgUrl.StartsWith("/uploads/"))
                                DeleteLocalFile(img.ImgUrl);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete movie image file {ImgUrl} for movie {MovieId}", img.ImgUrl, id);
                        }

                        _context.MovieImgs.Remove(img);
                    }
                }

                // Remove many-to-many links (MovieActors, MovieCategories)
                if (movie.MovieActors != null && movie.MovieActors.Any())
                    _context.MovieActors.RemoveRange(movie.MovieActors);

                if (movie.MovieCategories != null && movie.MovieCategories.Any())
                    _context.MovieCategories.RemoveRange(movie.MovieCategories);

                // Finally remove the movie entity
                _context.Movies.Remove(movie);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Movie deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                await tx.RollbackAsync();
                _logger.LogError(dbEx, "DB error while deleting movie {MovieId}", id);

                // Provide a friendly message; include more details in logs
                TempData["Error"] = "Unable to delete the movie due to database constraints. Ensure related records (bookings, links) are removed first.";
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Unexpected error while deleting movie {MovieId}", id);
                TempData["Error"] = "An unexpected error occurred while deleting the movie.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }


        // ----------------- Helper methods -----------------

        private async Task PopulateVmSelects(MovieFormViewModel vm)
        {
            vm.Cinemas = await _context.Cinemas.OrderBy(c => c.Name).Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == vm.CinemaId)).ToListAsync();
            vm.Categories = await _context.Categories.OrderBy(c => c.Name).Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == vm.CategoryId)).ToListAsync();
            vm.Actors = await _context.Actors.OrderBy(a => a.FirstName)
                .Select(a => new SelectListItem($"{a.FirstName} {a.LastName}", a.Id.ToString(), vm.SelectedActorIds.Contains(a.Id))).ToListAsync();
        }

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

        // File validation: ensures allowed extension and size
        private bool ValidateFiles(IFormFile? poster, IEnumerable<IFormFile>? images)
        {
            if (poster != null && poster.Length > 0)
            {
                if (!IsAllowedFile(poster))
                {
                    ModelState.AddModelError("PosterFile",
                $"Poster must be an image ({string.Join(", ", _allowedExtensions)}) and <= {_maxFileBytes / (1024 * 1024)} MB.");
                    return false;
                }
            }

            if (images != null)
            {
                foreach (var f in images)
                {
                    if (f == null || f.Length == 0) continue;
                    if (!IsAllowedFile(f))
                    {
                        ModelState.AddModelError("UploadedImages",
                     $"Uploaded images must be images ({string.Join(", ", _allowedExtensions)}) and each <= {_maxFileBytes / (1024 * 1024)} MB.");
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsAllowedFile(IFormFile f)
        {
            var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext)) return false;
            if (f.Length <= 0 || f.Length > _maxFileBytes) return false;
            return true;
        }

        // Save file to wwwroot/uploads/<folder> and return public url (/uploads/<folder>/<filename>)
        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            await using var fs = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fs);
            return $"/uploads/{folder}/{fileName}";
        }

        // Delete local file and log if failed
        private bool DeleteLocalFile(string publicPath)
        {
            try
            {
                var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var trimmed = publicPath.TrimStart('/');
                var path = Path.Combine(root, trimmed.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                _logger.LogWarning("File to delete not found: {Path}", path);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete local file {PublicPath}", publicPath);
                return false;
            }
        }
    }
}

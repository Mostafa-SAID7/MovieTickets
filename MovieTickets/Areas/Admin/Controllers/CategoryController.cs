using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Areas.Admin.ViewModels;
using MovieTickets.Data;
using MovieTickets.Models;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(MovieDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Index (List all categories)
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.MovieCategories)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .Include(c => c.MovieCategories)
                    .ThenInclude(mc => mc.Movie)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // Create GET
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            try
            {
                var category = new Category { Name = vm.Name };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Category created.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category.");
                ModelState.AddModelError("", "Error creating category: " + ex.Message);
                return View(vm);
            }
        }

        // Edit GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var vm = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(vm);
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return NotFound();

            try
            {
                existing.Name = vm.Name;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Category updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error editing category {CategoryId}", id);
                ModelState.AddModelError("", "The category was modified by another user. Please try again.");
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to edit category {CategoryId}", id);
                ModelState.AddModelError("", "Error updating category: " + ex.Message);
                return View(vm);
            }
        }

        // Delete GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.Categories
                .Include(c => c.MovieCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // Delete POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.MovieCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            if (category.MovieCategories.Any())
            {
                TempData["Error"] = "Cannot delete category because it is linked to movies. Remove links first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Category deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category {CategoryId}", id);
                TempData["Error"] = "Error deleting category.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}

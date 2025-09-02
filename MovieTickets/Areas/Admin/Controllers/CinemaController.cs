using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using MovieTickets.Models;
using MovieTickets.Areas.Admin.ViewModels;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CinemaController(MovieDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Cinema
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas.ToListAsync();
            return View(cinemas);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _context.Cinemas.FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        // GET: Create
        public IActionResult Create() => View(new CinemaVM());

        // POST: Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            string? fileName = null;

            if (vm.LogoFile != null && vm.LogoFile.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "uploads/cinemas");
                Directory.CreateDirectory(uploadDir);

                fileName = Guid.NewGuid() + Path.GetExtension(vm.LogoFile.FileName);
                string filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.LogoFile.CopyToAsync(stream);
                }
            }

            var cinema = new Cinema
            {
                Name = vm.Name,
                Description = vm.Description,
                Address = vm.Address,
                CinemaLogo = fileName != null ? $"/uploads/cinemas/{fileName}" : null
            };

            _context.Add(cinema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            var vm = new CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Description = cinema.Description,
                Address = cinema.Address,
                CinemaLogo = cinema.CinemaLogo
            };
            return View(vm);
        }

        // POST: Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CinemaVM vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            if (vm.LogoFile != null && vm.LogoFile.Length > 0)
            {
                string uploadDir = Path.Combine(_env.WebRootPath, "uploads/cinemas");
                Directory.CreateDirectory(uploadDir);

                string newFileName = Guid.NewGuid() + Path.GetExtension(vm.LogoFile.FileName);
                string filePath = Path.Combine(uploadDir, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.LogoFile.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(cinema.CinemaLogo))
                {
                    string oldFile = Path.Combine(_env.WebRootPath, cinema.CinemaLogo.TrimStart('/'));
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                cinema.CinemaLogo = $"/uploads/cinemas/{newFileName}";
            }

            cinema.Name = vm.Name;
            cinema.Description = vm.Description;
            cinema.Address = vm.Address;

            _context.Update(cinema);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.Movies)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null) return NotFound();

            if (cinema.Movies.Any())
            {
                TempData["Error"] = "Cannot delete this cinema because it has movies assigned.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(cinema.CinemaLogo))
            {
                string oldFile = Path.Combine(_env.WebRootPath, cinema.CinemaLogo.TrimStart('/'));
                if (System.IO.File.Exists(oldFile))
                    System.IO.File.Delete(oldFile);
            }

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cinema deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}

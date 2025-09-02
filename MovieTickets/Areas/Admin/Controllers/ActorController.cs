using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Areas.Admin.ViewModels;
using MovieTickets.Data;
using MovieTickets.Models;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly MovieDbContext _context;

        public ActorController(MovieDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Actor
        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors.ToListAsync();
            return View(actors);
        }

        // GET: Admin/Actor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }

        // GET: Admin/Actor/Create
        public IActionResult Create()
        {
            return View(new ActorFormViewModel());
        }

        // POST: Admin/Actor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var actor = new Actor
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Name = vm.Name,
                Bio = vm.Bio,
                ProfilePicture = vm.ProfilePicture,
                News = vm.News
            };

            _context.Add(actor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Actor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();

            var vm = new ActorFormViewModel
            {
                Id = actor.Id,
                FirstName = actor.FirstName,
                LastName = actor.LastName,
                Name = actor.Name,
                Bio = actor.Bio,
                ProfilePicture = actor.ProfilePicture,
                News = actor.News
            };

            return View(vm);
        }

        // POST: Admin/Actor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActorFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(vm);

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();

            actor.FirstName = vm.FirstName;
            actor.LastName = vm.LastName;
            actor.Name = vm.Name;
            actor.Bio = vm.Bio;
            actor.ProfilePicture = vm.ProfilePicture;
            actor.News = vm.News;

            _context.Update(actor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Actor/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        // POST: Admin/Actor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors
                .Include(a => a.MovieActors)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

            try
            {
                // First remove related MovieActors to avoid FK constraint errors
                if (actor.MovieActors.Any())
                {
                    _context.MovieActors.RemoveRange(actor.MovieActors);
                }

                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the error (you can use a logger here)
                var inner = ex.InnerException?.Message;
                ModelState.AddModelError("", $"Unable to delete actor. Error: {inner}");
                return View("Delete", actor);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

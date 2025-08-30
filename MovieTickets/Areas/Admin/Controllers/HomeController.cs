using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly MovieDbContext _context;

        public HomeController(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalMovies = await _context.Movies.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            ViewBag.TotalMovies = totalMovies;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}

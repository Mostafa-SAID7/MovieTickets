using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MovieTickets.Data;
using MovieTickets.Areas.Admin.ViewModels;

namespace MovieTickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly MovieDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;

        public HomeController(MovieDbContext context, ILogger<HomeController> logger, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Key for cache
                string cacheKey = "DashboardStats";

                // Check if data already cached
                if (!_cache.TryGetValue(cacheKey, out DashboardViewModel model))
                {
                    model = new DashboardViewModel
                    {
                        TotalMovies = await _context.Movies.CountAsync(),
                        TotalCategories = await _context.Categories.CountAsync(),
                        TotalUsers = await _context.Users.CountAsync(),
                        TotalBookings = await _context.Bookings.CountAsync(),
                        TotalShowtimes = await _context.Showtimes.CountAsync()
                    };

                    // Set cache options (expire after 1 minute)
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

                    _cache.Set(cacheKey, model, cacheOptions);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading dashboard data.");
                TempData["ErrorMessage"] = "An error occurred while loading dashboard data.";
                return View(new DashboardViewModel());
            }
        }
    }
}

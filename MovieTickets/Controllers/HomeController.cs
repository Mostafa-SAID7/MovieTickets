using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTickets.Data;
using System.Threading.Tasks;
using System.Linq;
using MovieTickets.Models;

namespace MovieTickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieDbContext _context;

        public HomeController(MovieDbContext context)
        {
            _context = context;
        }

        // 🏠 Home Page
        public  IActionResult Index()
        {
            //var query = _context.Movies
            //    .Include(m => m.MovieCategories)
            //    .ThenInclude(mc => mc.Category)
            //    .AsQueryable();

            // 🔍 Search
           

            // 🎭 Filter by category
          

            // 📊 Pagination
         

            // 🎥 Featured movies (latest)
           

            // 🎬 Upcoming movies
         

            // 🎁 Offers (example: active promotions)
            

       

            return View();
        }

        // ℹ️ About Us
        public IActionResult About()
        {
            return View();
        }

        // 📞 Contact Us
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(string name, string email, string message)
        {
            // TODO: Save to DB or send email
            TempData["Success"] = "Your message has been sent successfully!";
            return RedirectToAction("Contact");
        }

        // ❓ FAQ
        public IActionResult FAQ()
        {
            return View();
        }

        // 🔒 Privacy Policy
        public IActionResult Privacy()
        {
            return View();
        }

        // 📜 Terms & Conditions
        public IActionResult Terms()
        {
            return View();
        }

        // 💸 Refund Policy
        public IActionResult RefundPolicy()
        {
            return View();
        }

        // 📰 Blog
        public async Task<IActionResult> Blog()
        {
            var posts = await _context.Blogs
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        // 👔 Careers
        public async Task<IActionResult> Careers()
        {
            var jobs = await _context.Jobs
                .Where(j => j.IsActive)
                .ToListAsync();

            return View(jobs);
        }

        // 🛟 Support
        public IActionResult Support()
        {
            return View();
        }

        // 🗺️ Sitemap
        public IActionResult Sitemap()
        {
            return View();
        }

        // 🎁 Offers
        public async Task<IActionResult> Offers()
        {
            var offers = await _context.Offers
                .Where(o => o.IsActive && o.ExpiryDate >= DateTime.Now)
                .ToListAsync();

            return View(offers);
        }

        // 📣 Promotions
        public async Task<IActionResult> Promotions()
        {
            var promos = await _context.Promotions
                .Where(p => p.IsActive && p.ExpiryDate >= DateTime.Now)
                .ToListAsync();

            return View(promos);
        }

        // 🎟️ Events
        public async Task<IActionResult> Events()
        {
            var eventsList = await _context.Events
                .Where(e => e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .ToListAsync();

            return View(eventsList);
        }

        // 🚫 404 Page
        [Route("Home/Error404")]
        public IActionResult Error404()
        {
            return View();
        }
    }
}

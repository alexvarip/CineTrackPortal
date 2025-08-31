using CineTrackPortal.Data;
using CineTrackPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CineTrackPortal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int movieCount = _context.Movies.Count();
                int actorCount = _context.Actors.Count(); 
                int userCount = _context.Users.Count();
                ViewBag.MovieCount = movieCount;
                ViewBag.ActorCount = actorCount;
                ViewBag.UserCount = userCount;
                return View("IndexLoggedIn");
            }
            return View("Index");
        }

        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

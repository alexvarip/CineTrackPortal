using CineTrackPortal.Data;
using CineTrackPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace CineTrackPortal.Controllers
{
    [Authorize]
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        // GET: ActorsController/List
        public async Task<IActionResult> ListActors(string searchTerm, int pageIndex = 1)
        {
            var query = _context.Actors
                .Include(m => m.Movies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a =>
                    a.FirstName.Contains(searchTerm) ||
                    a.LastName.Contains(searchTerm));
            }

            var actors = await PaginatedList<ActorModel>.CreateAsync(
                query.OrderBy(m => m.FirstName), pageIndex, 10);

            ViewBag.SearchTerm = searchTerm;

            return View(actors);
        }


        [HttpGet]
        public async Task<IActionResult> SearchActors(string searchTerm, int pageIndex = 1)
        {
            var query = _context.Actors
                .Include(a => a.Movies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a =>
                    a.FirstName.Contains(searchTerm) ||
                    a.LastName.Contains(searchTerm));
            }

            var actors = await PaginatedList<ActorModel>.CreateAsync(
                query.OrderBy(a => a.FirstName), pageIndex, 10);

            ViewBag.SearchTerm = searchTerm;

            return PartialView("_ActorsTablePartial", actors);
        }


        // GET: ActorsController/Details/{Guid}
        public async Task<IActionResult> Details(Guid id)
        {
            var actor = await _context.Actors
               .Include(a => a.Movies)
               .FirstOrDefaultAsync(a => a.ActorId == id);

            if (actor == null)
                return NotFound();

            return View(actor);
        }


        // GET: ActorsController/Create
        public IActionResult Create()
        {
            PopulateMoviesDropDownList();
            return View();
        }


        // POST: ActorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorModel actor, Guid[] selectedMovies, List<MovieModel> NewMovies)
        {
            // Add selected existing movies
            if (selectedMovies != null)
            {
                actor.Movies = new List<MovieModel>();
                foreach (var movieId in selectedMovies)
                {
                    var movie = await _context.Movies.FindAsync(movieId);
                    if (movie != null)
                    {
                        actor.Movies.Add(movie);
                    }
                }
            }
            else
            {
                actor.Movies = new List<MovieModel>();
            }

            // Add new movies from the form
            if (NewMovies != null)
            {
                foreach (var newMovie in NewMovies)
                {
                    // Only add if both fields are provided
                    if (!string.IsNullOrWhiteSpace(newMovie.Title) && newMovie.Date != default)
                    {
                        newMovie.MovieId = Guid.NewGuid();
                        _context.Movies.Add(newMovie);
                        actor.Movies.Add(newMovie);
                    }
                }
            }

            bool hasExisting = actor.Movies != null && actor.Movies.Count > 0;
            bool hasNew = (NewMovies != null) && NewMovies.Any(a =>
                !string.IsNullOrWhiteSpace(a.Title));
            if (!hasExisting && !hasNew)
            {
                ModelState.AddModelError("", "Please select at least one existing movie or add a new one.");
                PopulateMoviesDropDownList(selectedMovies);
                return View(actor);
            }

            // Check for existing actor by Last Name (case-insensitive)
            var exists = await _context.Actors
                .AnyAsync(m => m.LastName.ToLower() == actor.LastName.ToLower());

            if (exists)
            {
                ModelState.AddModelError("Title", "An actor with this last name already exists.");
                PopulateMoviesDropDownList(selectedMovies);
                return View(actor);
            }

            if (ModelState.IsValid)
            {
                actor.ActorId = Guid.NewGuid();
                _context.Actors.Add(entity: actor);
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                PopulateMoviesDropDownList();
                return View(actor); // Return the same view to show the alert
            }

            // Repopulate dropdown if validation fails
            PopulateMoviesDropDownList(selectedMovies);
            return View(actor);
        }


        // GET: ActorsController/Edit/{Guid}
        public async Task<IActionResult> Edit(Guid id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();
            return View(actor);
        }


        // POST: ActorsController/Edit/{Guid}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ActorId, FirstName, LastName")] ActorModel actor)
        {
            if (id != actor.ActorId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(actor);

            // Only update FirstName and LastName, not navigation properties
            var actorToUpdate = await _context.Actors.FindAsync(id);
            if (actorToUpdate == null)
                return NotFound();

            actorToUpdate.FirstName = actor.FirstName;
            actorToUpdate.LastName = actor.LastName;

            try
            {
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                return View(actorToUpdate); // Return the view with the updated model and success flag
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Actors.Any(e => e.ActorId == id))
                    return NotFound();
                throw;
            }
        }


        // GET: ActorsController/Delete/Guid
        public async Task<IActionResult> Delete(Guid id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
                return NotFound();
            return View(actor);
        }


        // POST: ActorsController/Delete/Guid
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var actor = await _context.Actors
                    .Include(m => m.Movies)
                    .FirstOrDefaultAsync(m => m.ActorId == id);

            if (actor == null)
                return NotFound();

            // Remove only the association between the movie and its actors
            if (actor.Movies != null && actor.Movies.Any())
            {
                actor.Movies.Clear();
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
            ViewBag.EditSuccess = true;
            return View(actor);
        }


        // Helper to populate ViewBag.Movies for dropdown/multiselect
        private void PopulateMoviesDropDownList(IEnumerable<Guid>? selectedMovies = null)
        {
            var movies = _context.Movies
                .Select(a => new SelectListItem
                {
                    Value = a.MovieId.ToString(),
                    Text = a.Title + " - " + a.Date.ToLongDateString(),
                    Selected = selectedMovies != null && selectedMovies.Contains(a.MovieId)
                }).ToList();
            ViewBag.Movies = movies;
        }


    }
}
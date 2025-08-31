using CineTrackPortal.Data;
using CineTrackPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace CineTrackPortal.Controllers
{
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
        public async Task<IActionResult> ListActors(int pageIndex = 1)
        {
            var query = _context.Actors
                .Include(m => m.Movies)
                .OrderBy(m => m.FirstName)
                .AsNoTracking();

            var actors = await PaginatedList<ActorModel>.CreateAsync(query, pageIndex, PageSize);

            return View(actors);
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
            PopulateActorsDropDownList();
            return View();
        }


        // POST: ActorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorModel actor, Guid[] selectedMovies)
        {
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

            // Check for existing actor by Last Name (case-insensitive)
            var exists = await _context.Actors
                .AnyAsync(m => m.LastName.ToLower() == actor.LastName.ToLower());

            if (exists)
            {
                ModelState.AddModelError("Title", "An actor with this last name already exists.");
                PopulateActorsDropDownList(selectedMovies);
                return View(actor);
            }

            if (ModelState.IsValid)
            {
                actor.ActorId = Guid.NewGuid();
                _context.Actors.Add(entity: actor);
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                PopulateActorsDropDownList();
                return View(actor); // Return the same view to show the alert
            }

            // Repopulate dropdown if validation fails
            PopulateActorsDropDownList(selectedMovies);
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


        // Helper to populate ViewBag.Actors for dropdown/multiselect
        private void PopulateActorsDropDownList(IEnumerable<Guid>? selectedActors = null)
        {
            var actors = _context.Actors
                .Select(a => new SelectListItem
                {
                    Value = a.ActorId.ToString(),
                    Text = a.FirstName + " " + a.LastName,
                    Selected = selectedActors != null && selectedActors.Contains(a.ActorId)
                }).ToList();
            ViewBag.Actors = actors;
        }


    }
}
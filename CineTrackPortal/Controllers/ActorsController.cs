using CineTrackPortal.Data;
using CineTrackPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;


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
            return View();
        }


        // POST: ActorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorModel actor)
        {
            if (ModelState.IsValid)
            {
                actor.ActorId = Guid.NewGuid();
                _context.Actors.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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

            // Remove associated actors
            if (actor.Movies != null && actor.Movies.Any())
            {
                _context.Movies.RemoveRange(actor.Movies);
            }

            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListActors));
        }


    }
}
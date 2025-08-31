using CineTrackPortal.Data;
using CineTrackPortal.Models;
using CsvHelper;
using CsvHelper.Configuration;
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
        public async Task<IActionResult> Create(MovieModel movie)
        {
            if (ModelState.IsValid)
            {
                movie.MovieId = Guid.NewGuid();
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }


        // GET: ActorsController/Edit/{Guid}
        public async Task<IActionResult> Edit(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }


        // POST: ActorsController/Edit/{Guid}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MovieId, Title, Date")] MovieModel movie)
        {
            if (id != movie.MovieId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(movie);

            // Only update Title and Date, not navigation properties
            var movieToUpdate = await _context.Movies.FindAsync(id);
            if (movieToUpdate == null)
                return NotFound();

            movieToUpdate.Title = movie.Title;
            movieToUpdate.Date = movie.Date;

            try
            {
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                return View(movieToUpdate); // Return the view with the updated model and success flag
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Movies.Any(e => e.MovieId == id))
                    return NotFound();
                throw;
            }
        }


        // GET: ActorsController/Delete/Guid
        public async Task<IActionResult> Delete(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }


        // POST: ActorsController/Delete/Guid
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var movie = await _context.Movies
                    .Include(m => m.Actors)
                    .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
                return NotFound();

            // Remove associated actors
            if (movie.Actors != null && movie.Actors.Any())
            {
                _context.Actors.RemoveRange(movie.Actors);
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListActors));
        }


    }
}
using CineTrackPortal.Data;
using CineTrackPortal.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;


namespace CineTrackPortal.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        // GET: MoviesController/List
        public async Task<IActionResult> ListMovies(int pageIndex = 1)
        {
            var query = _context.Movies
                .Include(m => m.Actors)
                .OrderBy(m => m.Title)
                .AsNoTracking();

            var movies = await PaginatedList<MovieModel>.CreateAsync(query, pageIndex, PageSize);

            return View(movies);
        }


        // GET: MoviesController/Details/{Guid}
        public async Task<IActionResult> Details(Guid id)
        {
            var movie = await _context.Movies
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
                return NotFound();

            // Read CSV at runtime and find matching row
            var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "imdb_movies_mini.csv");
            dynamic? csvRow = null;

            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    // Match by title (and optionally date)
                    if (string.Equals((string)record.names, movie.Title, StringComparison.OrdinalIgnoreCase))
                    {
                        csvRow = record;
                        break;
                    }
                }
            }

            ViewBag.CsvRow = csvRow;

            return View(movie);
        }


        // GET: MoviesController/Create
        public IActionResult Create()
        {
            PopulateActorsDropDownList();
            return View();
        }


        // POST: MoviesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieModel movie, Guid[] selectedActors, List<ActorModel> NewActors)
        {
            // Add selected existing actors
            if (selectedActors != null)
            {
                movie.Actors = new List<ActorModel>();
                foreach (var actorId in selectedActors)
                {
                    var actor = await _context.Actors.FindAsync(actorId);
                    if (actor != null)
                    {
                        movie.Actors.Add(actor);
                    }
                }
            }
            else
            {
                movie.Actors = new List<ActorModel>();
            }

            // Add new actors from the form
            if (NewActors != null)
            {
                foreach (var newActor in NewActors)
                {
                    // Only add if both names are provided
                    if (!string.IsNullOrWhiteSpace(newActor.FirstName) && !string.IsNullOrWhiteSpace(newActor.LastName))
                    {
                        newActor.ActorId = Guid.NewGuid();
                        _context.Actors.Add(newActor);
                        movie.Actors.Add(newActor);
                    }
                }
            }

            // Check for existing movie by title (case-insensitive)
            var exists = await _context.Movies
                .AnyAsync(m => m.Title.ToLower() == movie.Title.ToLower());

            if (exists)
            {
                ModelState.AddModelError("Title", "A movie with this title already exists.");
                PopulateActorsDropDownList(selectedActors);
                return View(movie);
            }

            if (ModelState.IsValid)
            {
                movie.MovieId = Guid.NewGuid();
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                PopulateActorsDropDownList();
                return View(movie); // Return the same view to show the alert
            }

            // Repopulate dropdown if validation fails
            PopulateActorsDropDownList(selectedActors);
            return View(movie);
        }



        // GET: MoviesController/Edit/{Guid}
        public async Task<IActionResult> Edit(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }


        // POST: MoviesController/Edit/{Guid}
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


        // GET: MoviesController/Delete/Guid
        public async Task<IActionResult> Delete(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }


        // POST: MoviesController/Delete/Guid
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var movie = await _context.Movies
                    .Include(m => m.Actors)
                    .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
                return NotFound();

            // Remove only the association between the movie and its actors
            if (movie.Actors != null && movie.Actors.Any())
            {
                movie.Actors.Clear();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            ViewBag.EditSuccess = true;
            return View(movie);
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
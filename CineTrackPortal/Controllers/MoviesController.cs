using CineTrackPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineTrackPortal.Data;
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

        // GET: MoviesController/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var movie = await _context.Movies
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        // GET: MoviesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MoviesController/Create
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

        // GET: MoviesController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        // POST: MoviesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MovieModel movie)
        {
            if (id != movie.MovieId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Entry(movie).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: MoviesController/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        // POST: MoviesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
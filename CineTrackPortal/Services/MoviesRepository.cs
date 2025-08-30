using CineTrackPortal.Data;
using CineTrackPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CineTrackPortal.Services
{
    public class MoviesRepository : IMoviesService
    {
        private readonly ApplicationDbContext _context;

        public MoviesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieModel>> GetAllAsync() =>
            await _context.Movies.ToListAsync();

        public async Task<MovieModel?> GetByIdAsync(int id) =>
            await _context.Movies.FindAsync(id);

        public async Task AddAsync(MovieModel movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MovieModel movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
        }
    }
}
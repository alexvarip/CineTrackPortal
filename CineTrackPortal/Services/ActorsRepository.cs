using CineTrackPortal.Data;
using CineTrackPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CineTrackPortal.Services
{
    public class ActorsRepository : IActorsService
    {
        private readonly ApplicationDbContext _context;

        public ActorsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActorModel>> GetAllAsync() =>
            await _context.Actors.ToListAsync();

        public async Task<ActorModel?> GetByIdAsync(int id) =>
            await _context.Actors.FindAsync(id);

        public async Task AddAsync(ActorModel actor)
        {
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ActorModel actor)
        {
            _context.Actors.Update(actor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
        }
    }
}

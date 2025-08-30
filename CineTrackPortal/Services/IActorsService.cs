using CineTrackPortal.Models;

namespace CineTrackPortal.Services
{
    public interface IActorsService
    {
        Task<IEnumerable<ActorModel>> GetAllAsync();
        Task<ActorModel?> GetByIdAsync(int id);
        Task AddAsync(ActorModel actor);
        Task UpdateAsync(ActorModel actor);
        Task DeleteAsync(int id);
    }
}

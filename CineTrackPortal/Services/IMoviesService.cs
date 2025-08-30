using CineTrackPortal.Models;

namespace CineTrackPortal.Services
{
    public interface IMoviesService
    {
        Task<IEnumerable<MovieModel>> GetAllAsync();
        Task<MovieModel?> GetByIdAsync(int id);
        Task AddAsync(MovieModel movie);
        Task UpdateAsync(MovieModel movie);
        Task DeleteAsync(int id);
    }
}

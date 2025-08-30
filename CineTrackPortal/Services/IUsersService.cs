using Microsoft.AspNetCore.Identity;

namespace CineTrackPortal.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<IdentityUser>> GetAllAsync();
        Task<IdentityUser?> GetByIdAsync(string id);
        Task<IdentityResult> CreateAsync(IdentityUser user, string password);
        Task<IdentityResult> UpdateAsync(IdentityUser user);
        Task<IdentityResult> DeleteAsync(string id);
    }
}

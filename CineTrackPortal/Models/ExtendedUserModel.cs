using Microsoft.AspNetCore.Identity;

namespace CineTrackPortal.Models
{
    public class ExtendedUserModel : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

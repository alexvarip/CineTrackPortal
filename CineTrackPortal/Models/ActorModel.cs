using System.ComponentModel.DataAnnotations;

namespace CineTrackPortal.Models
{
    public class ActorModel
    {
        [Key]
        public Guid ActorId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public ICollection<MovieModel> Movies { get; set; } = new List<MovieModel>();
    }
}

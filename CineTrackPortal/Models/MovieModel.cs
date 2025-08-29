using System.ComponentModel.DataAnnotations;

namespace CineTrackPortal.Models
{
    public class MovieModel
    {
        [Key]
        public Guid MovieId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public ICollection<ActorModel> Actors { get; set; } = new List<ActorModel>();

    }
}
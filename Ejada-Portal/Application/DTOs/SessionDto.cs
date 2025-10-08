using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }
        public string PresenterName { get; set; }
        public string SessionName { get; set; }

        public ICollection<SessionRatingDto> Ratings { get; set; } = new List<SessionRatingDto>();
    }
}

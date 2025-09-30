using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public string PresenterName { get; set; }
        public string SessionName { get; set; }
        public ICollection<SessionRating> Ratings { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SessionRating
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int RateSession { get; set; }

        [Range(1, 5)]
        public int RatePresenter { get; set; }

        public string Comments { get; set; } = string.Empty;

        public int SessionId { get; set; }
        public Session Session { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}

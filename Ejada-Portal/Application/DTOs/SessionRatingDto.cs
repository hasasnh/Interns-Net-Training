using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SessionRatingDto
    {
        public int Id { get; set; }
        public int SessionId { get; set; }

        public string SessionName { get; set; }
        public string PresenterName { get; set; }

        [Range(1, 5)]
        public int RateSession { get; set; }

        [Range(1, 5)]
        public int RatePresenter { get; set; }

        public string Comments { get; set; }
        public string UserId { get; set; }
        public string UserName { get; internal set; }
    }
}

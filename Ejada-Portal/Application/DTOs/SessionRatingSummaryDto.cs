using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class SessionRatingSummaryDto
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; } = string.Empty;
        public string PresenterName { get; set; } = string.Empty;
        public double AveragePresenterRate { get; set; }
        public double AverageSessionRate { get; set; }
        public int TotalRatings { get; set; }
    }
}

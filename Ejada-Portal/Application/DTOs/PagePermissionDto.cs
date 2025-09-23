using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PagePermissionDto
    {
        public string PageName { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}

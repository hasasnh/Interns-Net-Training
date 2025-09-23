using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PagePermission
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string RoleName { get; set; }
    }
}

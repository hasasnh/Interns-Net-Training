using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.IServices
{
    public  interface IJiraService
    {
        Task<object> GetSprintIssuesAsync(int boardId, int sprintId);
    }
}

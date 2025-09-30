using Application.ServiceManager;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ejada_Portal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JiraController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public JiraController(IServiceManager serviceManager)
        {
           _serviceManager = serviceManager;
        }

        [HttpGet("Issues")]
        public async Task<IActionResult> GetIssues(int boardId = 34, int sprintId = 34)
        {
            var result = await _serviceManager.JiraService.GetSprintIssuesAsync(boardId, sprintId);
            return Ok(result);
        }
    }
}

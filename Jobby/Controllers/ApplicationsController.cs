using Jobby.Data.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController(AppDbContext db) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllUserJobApplications()
        {
            return Ok("");
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllOrganizationJobApplications()
        {
            return Ok("");
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateJobApplication()
        {
            return Ok("");
        }
    }
}

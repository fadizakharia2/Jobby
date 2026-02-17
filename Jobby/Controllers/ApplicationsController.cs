using AutoMapper;
using Jobby.Data.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController(IMapper mapper ,AppDbContext db) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllUserJobApplications([FromServices] IAuthorizationHandler auth, CancellationToken ct)
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

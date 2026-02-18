using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Dtos.Application;
using Jobby.Dtos.Validations.ApplicationValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController(IMapper mapper ,AppDbContext db) : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApplicationDetailsDto>> GetAllUserJobApplications([FromQuery] int pageLimit, [FromQuery] int pageNumber,[FromServices] IAuthorizationService auth,UserManager<User> userManager, CancellationToken ct)
        {
            // step 1 fetch user from context
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("user token is invalid.");
            // step 2 fetch all user applications with pagniation
            pageLimit = pageLimit <= 0 ? 20 : pageLimit;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            var baseQuery = db.JobApplications
         .AsNoTracking()
         .Where(x => x.CandidateUserId == userId);

            var total = await baseQuery.CountAsync(ct);

            var apps = await baseQuery
                .Include(x => x.Job)
                .Include(x => x.CandidateUser)
                .Include(x => x.ApplicationFiles).ThenInclude(af => af.File)
                .Include(x => x.StatusHistory)
                .Include(x => x.Notes)
                .Include(x => x.Interviews)
                .OrderByDescending(x => x.AppliedAt)
                .Skip((pageNumber - 1) * pageLimit)
                .Take(pageLimit)
                .Select(x => new ApplicationDetailDto(
                    Job: x.Job,
                    Status: x.Status,
                    CandidateUser: x.CandidateUser,
                    Source: x.Source,
                    CoverLetter: x.CoverLetter,
                    ApplicationFiles: x.ApplicationFiles.ToList(),
                    ApplicationStatusHistory: x.StatusHistory.ToList(),
                    ApplicationNotes: x.Notes.ToList(),
                    Interviews: x.Interviews.ToList(),
                    AppliedAt: x.AppliedAt,
                    LastStatusChangedAt: x.LastStatusChangedAt
                ))
                .ToListAsync(ct);
            return Ok(new ApplicationDetailsDto(Data: apps, PageLimit:pageLimit, PageNumber: pageNumber, Total: total));
        }
        [Authorize]
        [HttpGet("{organizationId:guid}/{jobId:guid}")]
        public async Task<ActionResult<ApplicationDetailsDto>> GetAllOrganizationJobApplications([FromServices] IAuthorizationService auth, Guid organizationId,Guid jobId, [FromQuery] int pageLimit, [FromQuery] int pageNumber,CancellationToken ct)
        {
            // step 1 get logged in user
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("token is invalid.");
            // step 2 authorize user with requirement OrgRecruiter
           var authResult = await auth.AuthorizeAsync(User, organizationId, "OrgRecruiter");
            if (!authResult.Succeeded)
                return Forbid();

            //step 3 fetch jobApplications with organization id and jobId
            pageLimit = pageLimit <= 0 ? 20 : pageLimit;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            var baseQuery = db.JobApplications
       .AsNoTracking()
       .Where(x => x.OrganizationId == organizationId && x.JobId == jobId);

            var total = await baseQuery.CountAsync(ct);
            var apps = await baseQuery
              .Include(x => x.Job)
              .Include(x => x.CandidateUser)
              .Include(x => x.ApplicationFiles).ThenInclude(af => af.File)
              .Include(x => x.StatusHistory)
              .Include(x => x.Notes)
              .Include(x => x.Interviews)
              .OrderByDescending(x => x.AppliedAt)
              .Skip((pageNumber - 1) * pageLimit)
              .Take(pageLimit)
              .Select(x => new ApplicationDetailDto(
                  Job: x.Job,
                  Status: x.Status,
                  CandidateUser: x.CandidateUser,
                  Source: x.Source,
                  CoverLetter: x.CoverLetter,
                  ApplicationFiles: x.ApplicationFiles.ToList(),
                  ApplicationStatusHistory: x.StatusHistory.ToList(),
                  ApplicationNotes: x.Notes.ToList(),
                  Interviews: x.Interviews.ToList(),
                  AppliedAt: x.AppliedAt,
                  LastStatusChangedAt: x.LastStatusChangedAt
              ))
              .ToListAsync(ct);
            // step 4 return result
            return Ok(new ApplicationDetailsDto(Data: apps, PageLimit: pageLimit, PageNumber: pageNumber, Total: total));
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateJobApplication(ApplicationCreateDto req,IValidator<ApplicationCreateDto> validator,CancellationToken ct)
        {
          var validationResult =  await validator.ValidateAsync(req,ct);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return ValidationProblem(ModelState);
            }
            var userNameStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userNameStr, out var userId))
            {
                return Unauthorized();
            }
            var job = await db.Jobs.Where(x => x.Id == req.jobId).FirstOrDefaultAsync(ct);
            if (job == null)
            {
                return NotFound("job not found");
            }
            db.JobApplications.Add(new JobApplications() { CandidateUserId=userId, CoverLetter=req.CoverLetter,AppliedAt=DateTimeOffset.UtcNow,LastStatusChangedAt=DateTimeOffset.UtcNow,Status=ApplicationStatus.Applied,Source=req.Source,OrganizationId=job.OrganizationId});
           var result = await db.SaveChangesAsync(ct);

            return NoContent();
        }
    }
}

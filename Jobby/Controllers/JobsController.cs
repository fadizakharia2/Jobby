using System.Net.NetworkInformation;
using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Dtos.jobs;
using Jobby.Dtos.Validations.jobValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController(AppDbContext db, IMapper mapper) : ControllerBase
    {

        [Authorize]
        [HttpPost("{orgId:guid}")]
        public async Task<ActionResult<JobsDto>> CreateJobPosting(
            [FromRoute] Guid orgId,
            [FromBody] JobsCreateRequestDto req,
            [FromServices] IAuthorizationService auth,
            [FromServices] IValidator<JobsCreateRequestDto> validator,
            CancellationToken ct)
        {
            // 0) get current user id (Identity uses NameIdentifier)
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Invalid user id in token.");

            // 1) auth
            var authResult = await auth.AuthorizeAsync(User, orgId, "OrgAdmin");
            if (!authResult.Succeeded)
                return Forbid();

            // 2) validate body
            var validationResult = await validator.ValidateAsync(req, ct);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return ValidationProblem(ModelState);
            }

            // 3) ensure org exists
            var orgExists = await db.Organizations.AnyAsync(o => o.Id == orgId, ct);
            if (!orgExists)
                return NotFound($"Organization {orgId} not found.");

            // (optional) ensure user exists to avoid FK crash if token is stale
            var userExists = await db.Users.AnyAsync(u => u.Id == userId, ct);
            if (!userExists)
                return Unauthorized("User no longer exists.");

            // 4) map + force FK fields
            var job = mapper.Map<Jobs>(req);
            job.OrganizationId = orgId;
            job.CreatedByUserId = userId;

            db.Jobs.Add(job);
            await db.SaveChangesAsync(ct);

            return Ok(mapper.Map<JobsDto>(job));
        }

        [Authorize]
        [HttpGet("{orgId:guid}")]
        public async Task<ActionResult<JobsDetailsResponseDto>> GetAllOrgJobs(Guid orgId, [FromQuery] int PageNumber, [FromQuery] int PageLimit, CancellationToken ct, [FromQuery] string? SortField = "CreatedAt", [FromQuery] string? SortValue = "", [FromQuery] string? q = "")
        {
           var query = db.Jobs.Where(x => x.Title.Contains(q) || (x.Description != null && x.Description.Contains(q))).Where(x=>x.OrganizationId == orgId);
            var term = q.Trim();
           query = ApplyJobsSort(query, SortField, SortValue);
            var total = await query.CountAsync(ct);

            var result = await query.Select(x=>mapper.Map<JobsDto>(x)).Skip((PageNumber - 1) * PageLimit).Take(PageLimit).ToListAsync(ct);

            return Ok(new JobsDetailsResponseDto(Data: result, Total: total, PageNumber: PageNumber, PageLimit: PageLimit));
        }

        [HttpGet]
        public async Task<ActionResult> GetJobs(
         IMapper mapper,
         [FromQuery] int PageNumber = 1,
         [FromQuery] int PageLimit = 10,
         [FromQuery] string SortField = "CreatedAt",
         [FromQuery] string SortValue = "",
         [FromQuery] string? q = "",
         CancellationToken ct = default)
        {
            var term = (q ?? "").Trim();

            IQueryable<Jobs> query = db.Jobs;

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.Title.Contains(term) ||
                    (x.Description != null && x.Description.Contains(term)));
            }

            query = ApplyJobsSort(query, SortField, SortValue);

            var total = await query.CountAsync(ct);

            var result = await query
                .Include(x=>x.Organization)
                .Skip((PageNumber - 1) * PageLimit)
                .Take(PageLimit)
                .Select(x => mapper.Map<JobsDto>(x)) // better: ProjectTo (but ok)
                .ToListAsync(ct);

            return Ok(new JobsDetailsResponseDto(result, PageLimit, PageNumber, total));
        }
        [Authorize]
        [HttpPatch("{orgId:guid}/{jobId:guid}")]
        public async Task<ActionResult<JobsDto>> UpdateJob(Guid jobId,Guid orgId, JobsUpdateRequestDto req,IValidator<JobsUpdateRequestDto> validator, [FromServices] IAuthorizationService auth, CancellationToken ct)
        {
            var authorizationResult = await auth.AuthorizeAsync(User, orgId, "OrgRecruiter");
            if (!authorizationResult.Succeeded)
            {
                return Forbid("user cannot access the following resource");
            }

            var validationResult = await validator.ValidateAsync(req);
            if (!validationResult.IsValid)
            {
                foreach(var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ValidationProblem(ModelState));
            }


            var job = await db.Jobs.FirstOrDefaultAsync(x=>x.Id == jobId,ct);

            job.Title = req.Title ?? job.Title;
            job.Description= req.Description ?? job.Description;
            job.Location= req.Location ?? job.Location;
            job.LocationType = req.LocationType ?? job.LocationType;
            job.EmploymentType= req.EmploymentType ?? job.EmploymentType;
             job.Status= req.Status ?? job.Status;
            job.SalaryMin= req.SalaryMin ?? job.SalaryMin;
            job.SalaryMax= req.SalaryMax ?? job.SalaryMax;
             job.Currency= req.Currency ?? job.Currency;
            job.UpdatedAt = DateTimeOffset.Now;
            await db.SaveChangesAsync(ct);


            var jobDto = mapper.Map<JobsDto>(job);
            return Ok(jobDto);
        }
        [Authorize]
        [HttpDelete("{orgId:guid}/{jobId:guid}")]
        public async Task<ActionResult> DeleteJob(Guid orgId, Guid jobId, [FromServices] IAuthorizationService auth, CancellationToken ct) {
            var result = await auth.AuthorizeAsync(User, orgId, "OrgRecruiter");
            if (!result.Succeeded)
            {
                return Forbid();
            }
            var job = await db.Jobs.FirstOrDefaultAsync(x=>x.Id == jobId);

            if (job == null)
            {
                return NotFound("Job does not exist!");
            }
            job.Status = JobStatus.Closed;
            await db.SaveChangesAsync(ct);
            return NoContent();
         }

        public static IQueryable<Jobs> ApplyJobsSort(IQueryable<Jobs> query, string? sortField, string? sortValue)
        {
            var desc = string.Equals(sortValue, "desc",StringComparison.OrdinalIgnoreCase);
            return (sortField ?? "createdat").ToLowerInvariant() switch {
                "title" => desc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
                "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                "location" => desc ? query.OrderByDescending(x => x.Location) : query.OrderBy(x => x.Location),
                "salarymin" => desc ? query.OrderByDescending(x => x.SalaryMin) : query.OrderBy(x => x.SalaryMin),
                "salarymax" => desc ? query.OrderByDescending(x => x.SalaryMax) : query.OrderBy(x => x.SalaryMax),
                "publishedat" => desc ? query.OrderByDescending(x => x.PublishedAt) : query.OrderBy(x => x.PublishedAt),
                "updatedat" => desc ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt),
                _ => desc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),

            };
        }
    }
}

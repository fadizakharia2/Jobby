using System.Net.NetworkInformation;
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
        public async Task <ActionResult<JobsDto>> CreateJobPosting(JobsCreateRequestDto req,[FromServices] IAuthorizationService auth, IValidator<JobsCreateRequestDto> validator, Guid orgId,CancellationToken ct)
        {
            
            // step 1 validate user and organization role
            var authResult =  await auth.AuthorizeAsync(User, orgId, "OrgAdmin");
            if (!authResult.Succeeded)
            {
                return Forbid("User is not allowed to access this organization");
            }
                // step 2 validate request body
          var validationResult = await validator.ValidateAsync(req);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ValidationProblem(ModelState));
            }
            // step 3 create entity
            db.Add(mapper.Map<Jobs>(req));
                // step 4 save entity
           var savedEntity = await db.SaveChangesAsync(ct);
               return Ok(mapper.Map<JobsDto>(savedEntity));
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
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult> FetJobs(Mapper mapper, [FromQuery] int PageNumber, [FromQuery] int PageLimit, CancellationToken ct, [FromQuery] string? SortField = "CreatedAt", [FromQuery] string? SortValue = "", [FromQuery] string? q = "")
        {
            var query = db.Jobs.Where(x => x.Title.Contains(q) || (x.Description != null && x.Description.Contains(q)));
            var term = q.Trim();
            query = ApplyJobsSort(query, SortField, SortValue);
            var total = await query.CountAsync(ct);

            var result = await query.Select(x => mapper.Map<JobsDto>(x)).Skip((PageNumber - 1) * PageLimit).Take(PageLimit).ToListAsync(ct);

            return Ok(new JobsDetailsResponseDto(Data: result, Total: total, PageNumber: PageNumber, PageLimit: PageLimit));
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

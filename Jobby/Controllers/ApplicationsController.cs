using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Dtos.Application;
using Jobby.Dtos.Mini;
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
.AsNoTracking()
.OrderByDescending(x => x.AppliedAt)
.Skip((pageNumber - 1) * pageLimit)
.Take(pageLimit)
.Select(x => new ApplicationDetailDto(
 Id: x.Id,
 Job: new JobMiniDto(
     Id: x.Job.Id,
     OrganizationId: x.Job.OrganizationId,
     Title: x.Job.Title,
     Location: x.Job.Location,
     LocationType: x.Job.LocationType,
     EmploymentType: x.Job.EmploymentType,
     Status: x.Job.Status,
     SalaryMin: x.Job.SalaryMin,
     SalaryMax: x.Job.SalaryMax,
     Currency: x.Job.Currency,
     PublishedAt: x.Job.PublishedAt
 ),
 Status: x.Status,
 Candidate: new CandidateMiniDto(
     Id: x.CandidateUser.Id,
     Email: x.CandidateUser.Email!,
     FirstName: x.CandidateUser.FirstName,
     LastName: x.CandidateUser.LastName
 ),
 Source: x.Source,
 CoverLetter: x.CoverLetter,
 Files: x.ApplicationFiles.Select(af => new ApplicationFileDto(
     FileId: af.FileId,
     OriginalName: af.File.OriginalName,
     ContentType: af.File.ContentType,
     SizeBytes: af.File.SizeBytes,
     FilePurpose: af.FilePurpose
 )).ToList(),
 StatusHistory: x.StatusHistory.Select(h => new ApplicationStatusHistoryDto(
     Id: h.Id,
     FromStatus: h.FromStatus,
     ToStatus: h.ToStatus,
     ChangedByUserId: h.ChangedByUserId,
     Reason: h.Reason,
     ChangedAt: h.ChangedAt
 )).ToList(),
 Notes: x.Notes.Select(n => new ApplicationNoteDto(
     Id: n.Id,
     AuthorUserId: n.AuthorUserId,
     NoteType: n.NoteType.ToString(),
     Visibility: n.Visibility.ToString(),
     Content: n.Content,
     CreatedAt: n.CreatedAt
 )).ToList(),
 Interviews: x.Interviews.Select(i => new InterviewDto(
     Id: i.Id,
     Stage: i.Stage.ToString(),
     StartsAt: i.StartsAt,
     EndsAt: i.EndsAt,
     Location: i.Location,
     MeetingUrl: i.MeetingUrl,
     Status: i.Status.ToString(),
     Feedback: i.Feedback,
     CreatedAt: i.CreatedAt
 )).ToList(),
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
    .AsNoTracking()
    .OrderByDescending(x => x.AppliedAt)
    .Skip((pageNumber - 1) * pageLimit)
    .Take(pageLimit)
    .Select(x => new ApplicationDetailDto(
        Id: x.Id,
        Job: new JobMiniDto(
            Id: x.Job.Id,
            OrganizationId: x.Job.OrganizationId,
            Title: x.Job.Title,
            Location: x.Job.Location,
            LocationType: x.Job.LocationType,
            EmploymentType: x.Job.EmploymentType,
            Status: x.Job.Status,
            SalaryMin: x.Job.SalaryMin,
            SalaryMax: x.Job.SalaryMax,
            Currency: x.Job.Currency,
            PublishedAt: x.Job.PublishedAt
        ),
        Status: x.Status,
        Candidate: new CandidateMiniDto(
            Id: x.CandidateUser.Id,
            Email: x.CandidateUser.Email!,
            FirstName: x.CandidateUser.FirstName,
            LastName: x.CandidateUser.LastName
        ),
        Source: x.Source,
        CoverLetter: x.CoverLetter,
        Files: x.ApplicationFiles.Select(af => new ApplicationFileDto(
            FileId: af.FileId,
            OriginalName: af.File.OriginalName,
            ContentType: af.File.ContentType,
            SizeBytes: af.File.SizeBytes,
            FilePurpose: af.FilePurpose
        )).ToList(),
        StatusHistory: x.StatusHistory.Select(h => new ApplicationStatusHistoryDto(
            Id: h.Id,
            FromStatus: h.FromStatus,
            ToStatus: h.ToStatus,
            ChangedByUserId: h.ChangedByUserId,
            Reason: h.Reason,
            ChangedAt: h.ChangedAt
        )).ToList(),
        Notes: x.Notes.Select(n => new ApplicationNoteDto(
            Id: n.Id,
            AuthorUserId: n.AuthorUserId,
            NoteType: n.NoteType.ToString(),
            Visibility: n.Visibility.ToString(),
            Content: n.Content,
            CreatedAt: n.CreatedAt
        )).ToList(),
        Interviews: x.Interviews.Select(i => new InterviewDto(
            Id: i.Id,
            Stage: i.Stage.ToString(),
            StartsAt: i.StartsAt,
            EndsAt: i.EndsAt,
            Location: i.Location,
            MeetingUrl: i.MeetingUrl,
            Status: i.Status.ToString(),
            Feedback: i.Feedback,
            CreatedAt: i.CreatedAt
        )).ToList(),
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

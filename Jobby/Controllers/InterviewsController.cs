using System.Security.Claims;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Dtos.Interviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/applications/{applicationId:guid}/interviews")]
public class InterviewsController(AppDbContext db,IMapper mapper) : ControllerBase
{
  

        [HttpPost]
    public async Task<IActionResult> Schedule(
        [FromServices] IAuthorizationService auth,
        [FromRoute] Guid applicationId,
        [FromBody] InterviewCreateReqDto dto,
        CancellationToken ct)
    {
                if (dto.ApplicationId != applicationId)
            return BadRequest("ApplicationId in body must match route applicationId.");
        var application = await db.JobApplications.FirstOrDefaultAsync(x=>x.Id == applicationId, ct);
        if (application == null)
            return BadRequest("Application does not exist.");
                var authResult = await auth.AuthorizeAsync(User, application.OrganizationId, "OrgRecruiter");
        if (!authResult.Succeeded) return Forbid();

                var exists = await db.JobApplications.AnyAsync(a => a.Id == applicationId, ct);
        if (!exists) return NotFound("Application not found.");

        var scheduledByUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(scheduledByUserIdStr) || !Guid.TryParse(scheduledByUserIdStr, out var scheduledByUserId))
            return Unauthorized("Invalid user id.");

        var interview = mapper.Map<Interview>(dto);
        interview.ScheduledByUserId = scheduledByUserId;
        interview.ApplicationId = applicationId;
        interview.Status = InterviewStatus.Scheduled;

        db.Interviews.Add(interview);
        await db.SaveChangesAsync(ct);

        var res = mapper.Map<InterviewDetailResDto>(interview);

        return CreatedAtAction(nameof(GetAll), new { applicationId }, res);
    }

        [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IAuthorizationService auth,
        [FromRoute] Guid applicationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageLimit = 20,
        CancellationToken ct = default)
    {
        if (applicationId == Guid.Empty)
            return BadRequest("application id is empty");
        var applications =await db.JobApplications.FirstOrDefaultAsync(x=>x.Id == applicationId,ct);
        if (applications == null)
            return NotFound("application not found");
                var authResult = await auth.AuthorizeAsync(User, new Dictionary<string, Guid> { ["applicationId"] = applicationId, ["organizationId"]=applications.OrganizationId}, "OrgCandidate");
        if (!authResult.Succeeded) return Forbid();

        if (pageNumber < 1) pageNumber = 1;
        if (pageLimit < 1) pageLimit = 20;
        if (pageLimit > 100) pageLimit = 100;

        var baseQuery = db.Interviews
            .AsNoTracking()
            .Where(i => i.ApplicationId == applicationId)
            .OrderBy(i => i.StartsAt);

        var total = await baseQuery.CountAsync(ct);

        var data = await baseQuery
            .Skip((pageNumber - 1) * pageLimit)
            .Take(pageLimit)
            .ProjectTo<InterviewDetailResDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Ok(new InterviewDetailsResDto(data, pageNumber, pageLimit, total));
    }

        [HttpPatch("{interviewId:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] IAuthorizationService auth,
        [FromRoute] Guid applicationId,
        [FromRoute] Guid interviewId,
        [FromBody] InterviewUpdateReqDto dto,
        CancellationToken ct)
    {

                var application = await db.JobApplications.FirstOrDefaultAsync(x => x.Id == applicationId, ct);
        if (application == null)
            return BadRequest("Application does not exist.");
                var authResult = await auth.AuthorizeAsync(User, application.OrganizationId, "OrgRecruiter");
        if (!authResult.Succeeded) return Forbid();

        var interview = await db.Interviews
            .FirstOrDefaultAsync(i => i.Id == interviewId && i.ApplicationId == applicationId, ct);

        if (interview is null) return NotFound("Interview not found.");

        if (interview.Status is InterviewStatus.Cancelled or InterviewStatus.Completed)
            return BadRequest("Cannot update a cancelled or completed interview.");

        mapper.Map(dto, interview);

                
        await db.SaveChangesAsync(ct);

        return Ok(mapper.Map<InterviewDetailResDto>(interview));
    }

        [HttpDelete("{interviewId:guid}")]
    public async Task<IActionResult> Cancel(
        [FromServices] IAuthorizationService auth,
        [FromRoute] Guid applicationId,
        [FromRoute] Guid interviewId,
        [FromBody] InterviewCancelReqDto dto,
        CancellationToken ct)
    {
        var application = await db.JobApplications.FirstOrDefaultAsync(x => x.Id == applicationId, ct);
        if (application == null)
            return BadRequest("Application does not exist.");
                var authResult = await auth.AuthorizeAsync(User, application.OrganizationId, "OrgRecruiter");
        if (!authResult.Succeeded) return Forbid();

        var interview = await db.Interviews
            .FirstOrDefaultAsync(i => i.Id == interviewId && i.ApplicationId == applicationId, ct);

        if (interview is null) return NotFound("Interview not found.");

        if (interview.Status == InterviewStatus.Completed)
            return BadRequest("Cannot cancel a completed interview.");

        interview.Status = InterviewStatus.Cancelled;

                interview.Feedback = string.IsNullOrWhiteSpace(dto.Reason)
            ? interview.Feedback
            : $"CANCELLED: {dto.Reason}";

                
        await db.SaveChangesAsync(ct);

        return Ok(mapper.Map<InterviewDetailResDto>(interview));
    }

        [HttpPost("{interviewId:guid}/complete")]
    public async Task<IActionResult> Complete(
        [FromServices] IAuthorizationService auth,
        [FromRoute] Guid applicationId,
        [FromRoute] Guid interviewId,
        [FromBody] InterviewCompleteReqDto dto,
        CancellationToken ct)
    {
        var application = await db.JobApplications.FirstOrDefaultAsync(x => x.Id == applicationId, ct);
        if (application == null)
            return BadRequest("Application does not exist.");
                var authResult = await auth.AuthorizeAsync(User, application.OrganizationId, "OrgRecruiter");
        if (!authResult.Succeeded) return Forbid();

        var interview = await db.Interviews
            .FirstOrDefaultAsync(i => i.Id == interviewId && i.ApplicationId == applicationId, ct);

        if (interview is null) return NotFound("Interview not found.");

        if (interview.Status == InterviewStatus.Cancelled)
            return BadRequest("Cannot complete a cancelled interview.");

        interview.Status = InterviewStatus.Completed;
        if (!string.IsNullOrWhiteSpace(dto.Feedback))
            interview.Feedback = dto.Feedback;

                
        await db.SaveChangesAsync(ct);

        return Ok(mapper.Map<InterviewDetailResDto>(interview));
    }
}
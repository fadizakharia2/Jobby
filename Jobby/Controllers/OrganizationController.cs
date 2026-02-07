using System.Security.Claims;
using System.Xml.Linq;
using AutoMapper;
using FluentValidation;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Dtos.Validations.Organization;
using Jobby.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Jobby.Dtos.OrganizationDtos.OrganizationDtos;
namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController(AppDbContext db,IMapper mapper) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateOrganization(IValidator<CreateOrganizationRequest> validator,CreateOrganizationRequest req,CancellationToken ct)
        {
          
            // step 1 validate request
            var validationResult =  await validator.ValidateAsync(req,ct);
            if (!validationResult.IsValid) {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ValidationProblem(ModelState));
            }
            // step 2 create organization
            var userId = GetUserIdOrUnauthorized(out var unauthorized);
            var entity = mapper.Map<Organization>(req);
            db.Organizations.Add(entity);
            entity.CreatedByUserId = userId;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            if (unauthorized is not null) return unauthorized;
            await db.SaveChangesAsync(ct);
            db.OrganizationMembers.Add(new OrganizationMembers
            {
                OrganizationId = entity.Id,
                UserId = userId,
                Role = "ADMIN"
            });
            await db.SaveChangesAsync(ct);

            // step 3 add creator as admin in organizationMembers table and create org-scoped role if needed
            return Ok(mapper.Map<OrganizationDto>(entity));
            // step 4 return organization details

        }
        [Authorize]
        [HttpPut("{orgId:guid}")]
        public async Task<ActionResult> UpdateOrganization(Guid orgId, [FromServices] IAuthorizationService authorizationService, IValidator<UpdateOrganizationRequest> validator, UpdateOrganizationRequest req, CancellationToken ct)
        {
            // step 1 validate request
            var validationResult =  await validator.ValidateAsync(req,ct);
            if (!validationResult.IsValid) {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ValidationProblem(ModelState));
            }
            // step 2 check if organization exists and user is admin
            var org = await db.Organizations.FindAsync(new object?[] { orgId }, ct);
            if (org is null) return NotFound();
            var result = await authorizationService.AuthorizeAsync(User, orgId, "OrgAdmin");

            if (!result.Succeeded)
                return Forbid();
            // step 3 update organization details
            org.Name = req.Name;
            org.UpdatedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
            // step 4 return updated organization details
            return Ok(mapper.Map<OrganizationDto>(org));
        }
        [Authorize]
        [HttpGet("{orgId:guid}")]
        public async Task<ActionResult> GetOrganization(Guid orgId, [FromServices] IAuthorizationService authorizationService, CancellationToken ct)
        {
            // step 1 check if organization exists and user is a member
            var org = await db.Organizations.FindAsync(new object?[] { orgId }, ct);
            if (org is null) return NotFound();
            var result = await authorizationService.AuthorizeAsync(User, orgId, "OrgRecruiter");
            if (!result.Succeeded)
                return Forbid();
            // step 2 return organization details
            return Ok(mapper.Map<OrganizationDto>(org));
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserOrganizations(CancellationToken ct)
        {
            // step 1 get user id
            var userId = GetUserIdOrUnauthorized(out var unauthorized);
            if (unauthorized is not null) return unauthorized;
            // step 2 get organizations where user is a member
            var orgs = await db.OrganizationMembers.Where(m => m.UserId == userId)
                .Include(m => m.Organization)
                .Select(m => mapper.Map<OrganizationDto>(m.Organization))
                .ToListAsync(ct);
            // step 3 return list of organizations
            return Ok(orgs);
        }
        private Guid GetUserIdOrUnauthorized(out UnauthorizedResult? unauthorized)
        {
            unauthorized = null;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                unauthorized = Unauthorized();
                return Guid.Empty;
            }

            return userId;
        }
    }
}


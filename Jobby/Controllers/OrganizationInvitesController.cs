using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using FluentValidation;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Dtos.OrganizationMembersDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationInvitesController(AppDbContext db,IMapper mapper) : ControllerBase
    {
        [Authorize]
        [HttpPost("{orgId:guid}/")]
        public async Task<ActionResult<OrganizationInvitesDto>> CreateInvite(Guid orgId,CreateInviteRequestDto req,[FromServices] IAuthorizationService auth,IValidator<CreateInviteRequestDto> validator)
        {
            // validation 
            var result = await validator.ValidateAsync(req);
            if (!result.IsValid) {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ValidationProblem(ModelState));
            }

           var authResult = await auth.AuthorizeAsync(User, orgId, "OrgAdmin");
            if (!authResult.Succeeded)
            {
               return Forbid();
            }
           var createdInvite= mapper.Map<CreateInviteRequestDto, OrganizationInvites>(req);
            var createdBy = createdInvite.CreatedByUserId = db.Find<User>(User.FindFirstValue(ClaimTypes.NameIdentifier))!.Id;
                createdInvite.OrganizationId = orgId;
                createdInvite.TokenHash = HashToken(GenerateToken());
                createdInvite.ExpiresAt = DateTime.UtcNow.AddDays(7);
            db.OrganizationInvites.Add(createdInvite);
            await db.SaveChangesAsync();
            var inviteDto = mapper.Map<OrganizationInvites, OrganizationInvitesDto>(createdInvite);
            return Ok(inviteDto);
         //    Guid CreatedByUserId,
         //    string TokenHash,
         //    Guid OrganizationId,
        }
        [Authorize]
        [HttpPost("/accept")]
        public async Task<ActionResult> AcceptInvite(AcceptInviteRequestDto req, IValidator<AcceptInviteRequestDto> validator,UserManager<User> userManager,CancellationToken ct)
        {
            var result = await validator.ValidateAsync(req);
            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ValidationProblem(ModelState));
            }
            var foundUser =await userManager.FindByIdAsync(ClaimTypes.NameIdentifier);
            if (foundUser == null)
            {
                return Unauthorized();
            }
            var OrgInvite = await db.OrganizationInvites.Include(o=>o.Organization).FirstOrDefaultAsync(x=>x.TokenHash == HashToken(req.token),ct);
            if (OrgInvite == null) 
                return NotFound();
            if (OrgInvite.ExpiresAt <= DateTime.UtcNow)
            {
                return BadRequest("This invite is already expired. Contact organization and ask for a new valid invite.");
            }
            if(OrgInvite.AcceptedAt is not null)
            {
                return BadRequest("This invite is already accepted.");
            }
            if(OrgInvite.Email != foundUser.Email)
            {
                return BadRequest("This invite is not meant for this email.");
            }
            OrgInvite.AcceptedAt = DateTime.UtcNow;
            if (!Guid.TryParse(foundUser.Id.ToString(), out var userId))
                throw new Exception("Unexpected error while parsing user.");

            db.OrganizationMembers.Add(new()
                {
                    Role = OrgInvite.InvitedRole,
                    OrganizationId = OrgInvite.OrganizationId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                });
            await db.SaveChangesAsync(ct);
            return NoContent();
        }
        [Authorize]
        [HttpPost("/decline")]
        public async Task<ActionResult> DeclineInvite(DeclineInviteRequestDto req,IValidator<DeclineInviteRequestDto> validator,CancellationToken ct)
        {
         var result = await validator.ValidateAsync(req, ct);
            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return ValidationProblem(ModelState);
            }
            
            var foundInvitation = await db.OrganizationInvites.Include(x=>x.Organization).FirstOrDefaultAsync(x => x.TokenHash == HashToken(req.token),ct);
            if (foundInvitation is null)
            {
                return NotFound("Invitation not found.");
            }
            var foundUser = await db.Users.FirstOrDefaultAsync(x=>x.Email == foundInvitation.Email,ct);
            if (foundUser is null)
            {
                return NotFound(
                    "user with the email does not exist!");
            }
            var userIdResult = Guid.TryParse(ClaimTypes.NameIdentifier, out var nameIdentifier);
            if (!userIdResult)
            {
                throw new Exception("Could not parse id.");
            }
            var currentUser = await db.Users.FirstOrDefaultAsync(x=>x.Id == nameIdentifier,ct);
            if(currentUser is null)
            {
                throw new Exception("user logged in is not found!");
            }
            if(currentUser.Email != foundInvitation.Email)
            {
                return Unauthorized("User email is not the same as the invitation email!");
            }
            foundInvitation.ExpiresAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return NoContent();
        }
        private static string GenerateToken()
        {
            // URL-safe-ish token: base64 without padding
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('=');
        }
        private static string HashToken(string token)
        {
            var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
    }
}

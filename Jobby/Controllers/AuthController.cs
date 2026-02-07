using System.Security.Claims;
using Jobby.Auth;
using Jobby.Data.context;
using Jobby.Data.entities;
using Jobby.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Jobby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        AppDbContext db,
        ITokenService tokens,
        IOptions<JwtOptions> options) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto req)
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, req.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");
            var (access, exp) = await tokens.CreateAccessTokenAsync(user);
            var refresh = tokens.CreateRefreshToken();
            db.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = tokens.HashRefreshToken(refresh), ExpiresAt = DateTimeOffset.UtcNow.AddDays(options.Value.RefreshTokenDays) });
            await db.SaveChangesAsync();
            return Ok(new AuthResponseDto(access, refresh, exp));
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto req)
        {
          var tokenHash = tokens.HashRefreshToken(req.RefreshToken);
            var stored = db.RefreshTokens.Include(rt => rt.User).FirstOrDefault(rt => rt.TokenHash == tokenHash);
            if (stored is null || !stored.IsActive) return Unauthorized();

            stored.RevokedAt= DateTimeOffset.UtcNow;
            var newRefresh = tokens.CreateRefreshToken();
            db.RefreshTokens.Add(new RefreshToken { UserId = stored.UserId, TokenHash = tokens.HashRefreshToken(newRefresh), ExpiresAt = DateTimeOffset.UtcNow.AddDays(options.Value.RefreshTokenDays) });
            var (access, exp) = await tokens.CreateAccessTokenAsync(stored.User);   
            await db.SaveChangesAsync();
            return Ok(new AuthResponseDto(access, newRefresh, exp));
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var activeTokens =await db.RefreshTokens.Where(rt => rt.UserId == userId && rt.RevokedAt == null).ToListAsync();
            foreach (var t in activeTokens)
            {
                t.RevokedAt = DateTimeOffset.UtcNow;
            }
            await db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto req)
        {
            var user = new User { FirstName = req.FirstName, LastName = req.LastName, Email = req.Email, UserName = req.Email };
            var result = await userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await userManager.AddToRoleAsync(user, "Candidate");
            var (access,exp) = await tokens.CreateAccessTokenAsync(user);
            var refresh = tokens.CreateRefreshToken();
            db.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = tokens.HashRefreshToken(refresh), ExpiresAt = DateTimeOffset.UtcNow.AddDays(options.Value.RefreshTokenDays) });
            await db.SaveChangesAsync();
            return Ok(new AuthResponseDto(access, refresh, exp));
        }
    }
}
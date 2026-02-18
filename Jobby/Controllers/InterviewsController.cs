using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/applications/{applicationId:guid}/interviews")]
public class InterviewsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Schedule() {
        return Ok("Coming Soon");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() {
        return Ok("Coming Soon");
    }

    [HttpPatch("{interviewId:guid}")]
    public async Task<IActionResult> Update() {
        return Ok("Coming Soon");
    }

    [HttpDelete("{interviewId:guid}")]
    public async Task<IActionResult> Cancel() {
        return Ok("Coming Soon");
    }

    [HttpPost("{interviewId:guid}/complete")]
    public async Task<IActionResult> Complete() {
        return Ok("Coming Soon");
    }
}
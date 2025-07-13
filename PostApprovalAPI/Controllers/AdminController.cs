using Microsoft.AspNetCore.Mvc;
using PostApprovalAPI.Services;

namespace PostApprovalAPI.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly PostService _postService;

    public AdminController(PostService postService)
    {
        _postService = postService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingPostsPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1 || pageSize < 1 || pageSize > 100 || page > pageSize)
                return BadRequest(new { message = "Invalid page or pageSize." });

            var posts = await _postService.GetPendingPostsPageAsync(page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                count = posts.Count,
                posts
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Server error", error = ex.Message });
        }
    }

    [HttpPut("posts/{id}/approve")]
    public async Task<IActionResult> ApprovePost(int id)
    {
        var success = await _postService.ApprovePostAsync(id);
        if (success)
            return Ok(new { message = "Post approved." });

        return BadRequest(new { message = "Post not found or not pending." });
    }

    [HttpPut("posts/{id}/reject")]
    public async Task<IActionResult> RejectPost(int id, [FromBody] RejectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return BadRequest(new { message = "Rejection reason required." });

        var success = await _postService.RejectPostAsync(id, request.Reason);
        if (success)
            return Ok(new { message = "Post rejected.\n" });

        return BadRequest(new { message = "Post not found or not pending." });
    }

    public class RejectRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostApprovalAPI.Models;
using PostApprovalAPI.Services;

namespace PostApprovalAPI.Controllers;

[ApiController]
[Route("posts")]
public class PostsController : ControllerBase
{
    private readonly PostService _postService;

    public PostsController(PostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Title and content are required." });

        // Validate user exists
        var user = await _postService.GetUserByIdAsync(request.CreatedById);
        if (user == null)
            return BadRequest(new { message = "Invalid CreatedById. User not found." });

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedById = user.Id,
            CreatedBy = user
        };

        var created = await _postService.AddPostAsync(post);

        return CreatedAtAction(nameof(GetPostStatus), new { id = created.Id }, new
        {
            message = "Post submitted. Waiting for review.\n",
            postId = created.Id
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetLatestPosts()
    {
        try
        {
            var posts = await _postService.GetLatestPostsAsync(limit: 10);
            if (posts == null || posts.Count == 0)
                return NotFound(new { message = "No posts found." });
            var result = posts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.ImageUrl,
                p.Status,
                p.ReviewReason,
                p.CreatedAt,
                CreatedBy = new
                {
                    p.CreatedBy.Id,
                    p.CreatedBy.Email
                }
            });
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Internal server error", error = e.Message });
        }
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetPostStatus(int id)
    {
        var post = await _postService.GetPostAsync(id);
        if (post == null)
            return NotFound(new { message = "Post not found." });

        return Ok(new
        {
            postId = post.Id,
            status = post.Status,
            reviewReason = post.ReviewReason,
            createdBy = post.CreatedBy.Email,
            createdAt  = post.CreatedAt,
            title = post.Title
        });
    }

    public class CreatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CreatedById { get; set; }
    }
}

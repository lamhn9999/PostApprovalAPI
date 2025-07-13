using Microsoft.EntityFrameworkCore;
using PostApprovalAPI.Data;
using PostApprovalAPI.Models;

namespace PostApprovalAPI.Services;

public class PostService
{
    private readonly PostDbContext _db;

    public PostService(PostDbContext db)
    {
        _db = db;
    }

    public async Task<Post> AddPostAsync(Post post)
    {
        post.Status = "pending";
        post.CreatedAt = DateTime.UtcNow;

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<List<Post>> GetLatestPostsAsync(int limit)
    {
        return await _db.Posts
            .Include(p => p.CreatedBy)
            .Where(p => p.Status == "approved")
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
    public async Task<Post?> GetPostAsync(int id)
    {
        return await _db.Posts
            .Include(p => p.CreatedBy)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ApprovePostAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null || post.Status != "pending") return false;

        post.Status = "approved";
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectPostAsync(int id, string reason)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null || post.Status != "pending") return false;

        post.Status = "rejected";
        post.ReviewReason = reason;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Post>> GetPendingPostsPageAsync(int page, int pageSize)
    {
        try
        {
            return await _db.Posts
                .Where(p => p.Status == "pending")
                .OrderBy(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error fetching pending posts: " + e.Message);
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _db.Users.FindAsync(userId);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await _db.Users.AnyAsync(u => u.Id == userId);
    }
}

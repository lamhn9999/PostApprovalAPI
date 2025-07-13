namespace PostApprovalAPI.Models;
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }

    public int CreatedById { get; set; }         // foreign key
    public User CreatedBy { get; set; } = null!; // navigation

    public string Status { get; set; } = "pending";
    public string? ReviewReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

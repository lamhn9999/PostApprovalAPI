namespace PostApprovalAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
using System.ComponentModel.DataAnnotations;

namespace Project.Models;

public class NewPostModel
{
    public bool IsComment { get; set; }
    
    [Required]
    public string Content { get; set; }
    public string CommentedPostId { get; set; }
}
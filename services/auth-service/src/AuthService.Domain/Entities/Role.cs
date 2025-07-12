using AuthService.Domain.Common;

namespace AuthService.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    
    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

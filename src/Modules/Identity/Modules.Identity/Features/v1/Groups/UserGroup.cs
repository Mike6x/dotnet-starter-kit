using FSH.Modules.Identity.Features.v1.Users;

namespace FSH.Modules.Identity.Features.v1.Groups;

public class UserGroup
{
    public string UserId { get; set; } = default!;
    public Guid GroupId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public string? AddedBy { get; set; }

    // Navigation properties
    public virtual FshUser? User { get; set; }
    public virtual Group? Group { get; set; }
}

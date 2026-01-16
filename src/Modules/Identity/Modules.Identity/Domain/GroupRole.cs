namespace FSH.Modules.Identity.Domain;

public class GroupRole
{
    public Guid GroupId { get; set; }
    public string RoleId { get; set; } = default!;

    // Navigation properties
    public virtual Group? Group { get; set; }
    public virtual FshRole? Role { get; set; }
}

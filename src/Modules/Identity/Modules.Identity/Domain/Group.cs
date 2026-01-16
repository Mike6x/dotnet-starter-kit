using FSH.Framework.Core.Domain;

namespace FSH.Modules.Identity.Domain;

public class Group : ISoftDeletable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSystemGroup { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }

    // ISoftDeletable implementation
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOnUtc { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation properties
    public virtual ICollection<GroupRole> GroupRoles { get; set; } = [];
    public virtual ICollection<UserGroup> UserGroups { get; set; } = [];
}

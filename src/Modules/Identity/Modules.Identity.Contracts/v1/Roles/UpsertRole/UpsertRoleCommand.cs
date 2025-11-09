namespace FSH.Modules.Identity.Contracts.v1.Roles.UpsertRole;

public class UpsertRoleCommand
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
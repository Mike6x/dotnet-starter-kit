using FSH.Framework.Core.Auth;

namespace FSH.Framework.Infrastructure.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequiredPermissionAttribute(string? requiredPermission, params string[]? additionalRequiredPermissions)
    : Attribute, IRequiredPermissionMetadata
{
    public HashSet<string> RequiredPermissions { get; } = [requiredPermission!, .. additionalRequiredPermissions];
    public string? RequiredPermission { get; }
    public string[]? AdditionalRequiredPermissions { get; }
}
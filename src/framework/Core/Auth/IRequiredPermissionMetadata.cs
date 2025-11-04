namespace FSH.Framework.Core.Auth;
public interface IRequiredPermissionMetadata
{
    HashSet<string> RequiredPermissions { get; }
}

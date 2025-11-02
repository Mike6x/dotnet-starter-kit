namespace FSH.Framework.Core.Identity.Users;
public interface IFshUser
{
    string? FirstName { get; }
    string? LastName { get; }
    Uri? ImageUrl { get; }
    bool IsActive { get; }
    string? RefreshToken { get; }
    DateTime RefreshTokenExpiryTime { get; }
}
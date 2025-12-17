namespace FSH.Playground.Blazor.Services;

/// <summary>
/// Service for managing and sharing user profile state across components.
/// When the profile is updated (e.g., in ProfileSettings), other components
/// like the layout header can subscribe to be notified of changes.
/// </summary>
public interface IUserProfileState
{
    string UserName { get; }
    string? UserEmail { get; }
    string? UserRole { get; }
    string? AvatarUrl { get; }

    /// <summary>
    /// Event raised when profile data changes.
    /// </summary>
    event Action? OnProfileChanged;

    /// <summary>
    /// Updates the profile state and notifies subscribers.
    /// </summary>
    void UpdateProfile(string userName, string? userEmail, string? userRole, string? avatarUrl);

    /// <summary>
    /// Clears the profile state (e.g., on logout).
    /// </summary>
    void Clear();
}

internal sealed class UserProfileState : IUserProfileState
{
    public string UserName { get; private set; } = "User";
    public string? UserEmail { get; private set; }
    public string? UserRole { get; private set; }
    public string? AvatarUrl { get; private set; }

    public event Action? OnProfileChanged;

    public void UpdateProfile(string userName, string? userEmail, string? userRole, string? avatarUrl)
    {
        UserName = userName;
        UserEmail = userEmail;
        UserRole = userRole;
        AvatarUrl = avatarUrl;
        OnProfileChanged?.Invoke();
    }

    public void Clear()
    {
        UserName = "User";
        UserEmail = null;
        UserRole = null;
        AvatarUrl = null;
        OnProfileChanged?.Invoke();
    }
}

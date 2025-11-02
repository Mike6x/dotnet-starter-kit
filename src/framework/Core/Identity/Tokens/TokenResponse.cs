namespace FSH.Framework.Core.Identity.Tokens;
public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    DateTime? AccessTokenExpiresAt = null);
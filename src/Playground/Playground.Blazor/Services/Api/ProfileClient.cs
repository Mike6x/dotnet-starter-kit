using System.Net.Http.Json;

namespace FSH.Playground.Blazor.Services.Api;

public sealed class ProfileClient
{
    private readonly HttpClient _httpClient;

    public ProfileClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("AuthApi");
    }

    public async Task<ProfileResponse?> GetProfileAsync(CancellationToken ct = default) =>
        await _httpClient.GetFromJsonAsync<ProfileResponse>("api/v1/identity/me", cancellationToken: ct);

    public async Task<bool> UpdateProfileAsync(ProfileUpdateRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsJsonAsync("api/v1/identity/me", request, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/v1/identity/me/change-password", request, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<string?> UploadProfileImageAsync(StreamContent content, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent
        {
            { content, "file", "profile-image" }
        };

        var response = await _httpClient.PostAsync("api/v1/identity/me/image", form, ct);
        if (!response.IsSuccessStatusCode) return null;
        var result = await response.Content.ReadFromJsonAsync<ImageUploadResponse>(cancellationToken: ct);
        return result?.ImageUrl;
    }

    public sealed class ProfileResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class ProfileUpdateRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public sealed class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public sealed class ImageUploadResponse
    {
        public string? ImageUrl { get; set; }
    }
}

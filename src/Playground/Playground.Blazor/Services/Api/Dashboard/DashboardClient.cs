using System.Net.Http.Json;

namespace FSH.Playground.Blazor.Services.Api.Dashboard;

public sealed class DashboardClient
{
    private readonly HttpClient _httpClient;

    public DashboardClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("AuthApi");
    }

    public async Task<Summary> GetSummaryAsync(CancellationToken ct = default)
    {
        // If no summary endpoint exists, this can be extended to call multiple endpoints.
        var response = await _httpClient.GetAsync("api/v1/identity/summary", ct);
        if (!response.IsSuccessStatusCode)
        {
            return new Summary();
        }

        var payload = await response.Content.ReadFromJsonAsync<Summary>(cancellationToken: ct);
        return payload ?? new Summary();
    }

    public sealed class Summary
    {
        public int Users { get; set; }
        public int Roles { get; set; }
        public int Tenants { get; set; }
        public int RecentAudits { get; set; }
    }
}

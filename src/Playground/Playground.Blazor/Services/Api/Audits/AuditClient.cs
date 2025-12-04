using Microsoft.AspNetCore.WebUtilities;

namespace FSH.Playground.Blazor.Services.Api.Audits;

public sealed class AuditClient
{
    private readonly HttpClient _httpClient;

    public AuditClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("AuthApi");
    }

    public async Task<PagedAudits> GetAuditsAsync(AuditFilter filter, CancellationToken ct = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["type"] = filter.Type,
            ["user"] = filter.User,
            ["correlationId"] = filter.CorrelationId,
            ["pageNumber"] = filter.PageNumber.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };

        var uri = QueryHelpers.AddQueryString("api/v1/audits", query!);
        var response = await _httpClient.GetAsync(uri, ct);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<PagedAudits>(cancellationToken: ct);
        return payload ?? new PagedAudits();
    }

    public sealed class AuditFilter
    {
        public string? Type { get; set; }
        public string? User { get; set; }
        public string? CorrelationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class PagedAudits
    {
        public List<AuditDto> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }
    }

    public sealed class AuditDto
    {
        public DateTime TimestampUtc { get; set; }
        public string Type { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? CorrelationId { get; set; }
        public string? TraceId { get; set; }
        public string? Payload { get; set; }
    }
}

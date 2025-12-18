using System.Text.Json;
using FSH.Framework.Blazor.UI.Theme;
using Microsoft.Extensions.Caching.Distributed;

namespace FSH.Playground.Blazor.Services;

/// <summary>
/// Factory for loading theme state, optimized for SSR scenarios.
/// </summary>
internal interface IThemeStateFactory
{
    Task<TenantThemeSettings> GetThemeAsync(string tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Redis-cached implementation of theme state factory.
/// Efficient for SSR pages that need theme data without full circuit.
/// </summary>
internal sealed class CachedThemeStateFactory : IThemeStateFactory
{
    private static readonly Uri ThemeEndpoint = new("/api/v1/tenants/theme", UriKind.Relative);

    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CachedThemeStateFactory> _logger;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(15);

    public CachedThemeStateFactory(
        IDistributedCache cache,
        HttpClient httpClient,
        ILogger<CachedThemeStateFactory> logger)
    {
        _cache = cache;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TenantThemeSettings> GetThemeAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"theme:{tenantId}";

        // Try to get from cache first (with error handling for Redis failures)
        try
        {
            var json = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (json is not null)
            {
                try
                {
                    var cached = JsonSerializer.Deserialize<TenantThemeSettings>(json);
                    if (cached is not null)
                    {
                        return cached;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached theme for tenant {TenantId}", tenantId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache unavailable, fetching theme directly for tenant {TenantId}", tenantId);
        }

        // Cache miss or deserialization failed - fetch from API
        try
        {
            var response = await _httpClient.GetAsync(ThemeEndpoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<TenantThemeApiDto>(cancellationToken);
                if (dto is not null)
                {
                    var settings = MapFromDto(dto);

                    // Try to cache for 15 minutes (fail silently if cache unavailable)
                    try
                    {
                        var serialized = JsonSerializer.Serialize(settings);
                        var options = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = _cacheExpiry
                        };
                        await _cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
                    }
                    catch (Exception cacheEx)
                    {
                        _logger.LogWarning(cacheEx, "Failed to cache theme, continuing without cache");
                    }

                    return settings;
                }
            }
            else
            {
                _logger.LogWarning("Failed to load tenant theme from API: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tenant theme for {TenantId}", tenantId);
        }

        // Fallback to default theme
        return TenantThemeSettings.Default;
    }

    private static TenantThemeSettings MapFromDto(TenantThemeApiDto dto)
    {
        var defaultSettings = TenantThemeSettings.Default;

        return new TenantThemeSettings
        {
            LightPalette = new PaletteSettings
            {
                Primary = dto.LightPalette?.Primary ?? defaultSettings.LightPalette.Primary,
                Secondary = dto.LightPalette?.Secondary ?? defaultSettings.LightPalette.Secondary,
                Tertiary = dto.LightPalette?.Tertiary ?? defaultSettings.LightPalette.Tertiary,
                Background = dto.LightPalette?.Background ?? defaultSettings.LightPalette.Background,
                Surface = dto.LightPalette?.Surface ?? defaultSettings.LightPalette.Surface,
                Error = dto.LightPalette?.Error ?? defaultSettings.LightPalette.Error,
                Warning = dto.LightPalette?.Warning ?? defaultSettings.LightPalette.Warning,
                Success = dto.LightPalette?.Success ?? defaultSettings.LightPalette.Success,
                Info = dto.LightPalette?.Info ?? defaultSettings.LightPalette.Info
            },
            DarkPalette = new PaletteSettings
            {
                Primary = dto.DarkPalette?.Primary ?? defaultSettings.DarkPalette.Primary,
                Secondary = dto.DarkPalette?.Secondary ?? defaultSettings.DarkPalette.Secondary,
                Tertiary = dto.DarkPalette?.Tertiary ?? defaultSettings.DarkPalette.Tertiary,
                Background = dto.DarkPalette?.Background ?? defaultSettings.DarkPalette.Background,
                Surface = dto.DarkPalette?.Surface ?? defaultSettings.DarkPalette.Surface,
                Error = dto.DarkPalette?.Error ?? defaultSettings.DarkPalette.Error,
                Warning = dto.DarkPalette?.Warning ?? defaultSettings.DarkPalette.Warning,
                Success = dto.DarkPalette?.Success ?? defaultSettings.DarkPalette.Success,
                Info = dto.DarkPalette?.Info ?? defaultSettings.DarkPalette.Info
            },
            BrandAssets = new BrandAssets
            {
                LogoUrl = dto.BrandAssets?.LogoUrl,
                LogoDarkUrl = dto.BrandAssets?.LogoDarkUrl,
                FaviconUrl = dto.BrandAssets?.FaviconUrl
            },
            Typography = new TypographySettings
            {
                FontFamily = dto.Typography?.FontFamily ?? defaultSettings.Typography.FontFamily,
                HeadingFontFamily = dto.Typography?.HeadingFontFamily ?? defaultSettings.Typography.HeadingFontFamily,
                FontSizeBase = dto.Typography?.FontSizeBase ?? defaultSettings.Typography.FontSizeBase,
                LineHeightBase = dto.Typography?.LineHeightBase ?? defaultSettings.Typography.LineHeightBase
            },
            Layout = new LayoutSettings
            {
                BorderRadius = dto.Layout?.BorderRadius ?? defaultSettings.Layout.BorderRadius,
                DefaultElevation = dto.Layout?.DefaultElevation ?? defaultSettings.Layout.DefaultElevation
            },
            IsDefault = dto.IsDefault
        };
    }
}

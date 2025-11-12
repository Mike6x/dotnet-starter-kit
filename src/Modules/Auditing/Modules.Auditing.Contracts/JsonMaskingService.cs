using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FSH.Modules.Auditing.Contracts;

/// <summary>
/// Simple masking by field-name convention or attributes.
/// </summary>
public sealed class JsonMaskingService : IAuditMaskingService
{
    private static readonly HashSet<string> _maskKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "secret", "token", "otp", "pin"
    };

    public object ApplyMasking(object payload)
    {
        try
        {
            var json = JsonSerializer.SerializeToNode(payload);
            if (json is null) return payload;
            MaskNode(json);
            return json;
        }
        catch
        {
            return payload; // safe fallback
        }
    }

    private void MaskNode(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            foreach (var kvp in obj.ToList())
            {
                if (ShouldMask(kvp.Key))
                {
                    obj[kvp.Key] = "****";
                }
                else if (kvp.Value is not null)
                {
                    MaskNode(kvp.Value);
                }
            }
        }
        else if (node is JsonArray arr)
        {
            foreach (var el in arr)
                if (el is not null) MaskNode(el);
        }
    }

    private static bool ShouldMask(string key)
        => _maskKeywords.Any(k => key.Contains(k, StringComparison.OrdinalIgnoreCase));

    // Optionally hash instead of mask — demonstration:
    private static string HashValue(string text)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes[..8]) + "…";
    }
}

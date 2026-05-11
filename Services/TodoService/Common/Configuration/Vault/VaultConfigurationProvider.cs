using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoService.Common.Configuration.Vault;

/// <summary>
/// Reads a single KV v2 secret from Vault at load time and exposes its
/// key/value pairs as configuration entries.
/// </summary>
internal sealed class VaultConfigurationProvider : ConfigurationProvider
{
    private readonly VaultConfigurationSource _source;

    public VaultConfigurationProvider(VaultConfigurationSource source)
    {
        _source = source;
    }

    public override void Load()
    {
        try
        {
            LoadCore().GetAwaiter().GetResult();
        }
        catch (Exception ex) when (_source.Optional)
        {
            // Swallow when Vault is optional (e.g. local dev where secrets
            // come from .env.local instead). Surface via stderr so the dev
            // notices but the app still boots.
            Console.Error.WriteLine($"[Vault] Skipping optional Vault source: {ex.Message}");
        }
    }

    private async Task LoadCore()
    {
        using var http = new HttpClient { BaseAddress = new Uri(_source.Address.TrimEnd('/') + "/") };
        http.DefaultRequestHeaders.Add("X-Vault-Token", _source.Token);

        var requestUri = $"v1/{_source.Mount}/data/{_source.Path}";
        using var response = await http.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var payload = await JsonSerializer.DeserializeAsync(stream, VaultJsonContext.Default.VaultKvV2Response)
            ?? throw new InvalidOperationException("Vault returned an empty response.");

        var secrets = payload.Data?.Data
            ?? throw new InvalidOperationException($"No secrets found at {_source.Mount}/{_source.Path}.");

        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in secrets)
        {
            // .NET env-var convention uses "__" as the section separator.
            // Normalize so callers can read both styles transparently.
            var normalized = key.Replace("__", ConfigurationPath.KeyDelimiter);
            data[normalized] = value;
        }

        Data = data;
    }
}

internal sealed class VaultKvV2Response
{
    [JsonPropertyName("data")] public VaultKvV2Data? Data { get; set; }
}

internal sealed class VaultKvV2Data
{
    [JsonPropertyName("data")] public Dictionary<string, string>? Data { get; set; }
}

[JsonSerializable(typeof(VaultKvV2Response))]
internal sealed partial class VaultJsonContext : JsonSerializerContext;

namespace TodoService.Common.Configuration.Vault;

/// <summary>
/// Configuration source that pulls a KV v2 secret from HashiCorp Vault and
/// exposes its key/value pairs as a standard <see cref="IConfiguration"/>
/// section. Keys using the ASP.NET Core double-underscore convention
/// (e.g. <c>ConnectionStrings__TodoDb</c>) are normalized to the colon
/// separator (<c>ConnectionStrings:TodoDb</c>).
/// </summary>
public sealed class VaultConfigurationSource : IConfigurationSource
{
    public required string Address { get; init; }
    public required string Token { get; init; }

    /// <summary>KV v2 mount name. Defaults to <c>secret</c>.</summary>
    public string Mount { get; init; } = "secret";

    /// <summary>Path under the mount, e.g. <c>todoservice/dev</c>.</summary>
    public required string Path { get; init; }

    /// <summary>
    /// If <c>true</c>, failures to reach Vault are swallowed (useful so the
    /// app can still start when another configuration source — e.g. env
    /// vars from <c>.env.local</c> — provides the values). Defaults to
    /// <c>false</c>.
    /// </summary>
    public bool Optional { get; init; }

    public IConfigurationProvider Build(IConfigurationBuilder builder) => new VaultConfigurationProvider(this);
}

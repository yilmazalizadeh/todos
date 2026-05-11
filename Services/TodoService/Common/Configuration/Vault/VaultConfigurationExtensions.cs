namespace TodoService.Common.Configuration.Vault;

public static class VaultConfigurationExtensions
{
    /// <summary>
    /// Adds a Vault KV v2 configuration source if <c>Vault:Address</c> is
    /// configured (via env vars, appsettings, etc.).
    ///
    /// Reads the following configuration keys (env var equivalents in parens):
    /// <list type="bullet">
    ///   <item><c>Vault:Address</c> (<c>VAULT_ADDR</c>) — required to activate.</item>
    ///   <item><c>Vault:Token</c> (<c>VAULT_TOKEN</c>) — required to activate.</item>
    ///   <item><c>Vault:Mount</c> (<c>VAULT_MOUNT</c>) — defaults to <c>secret</c>.</item>
    ///   <item><c>Vault:Path</c> (<c>VAULT_PATH</c>) — defaults to <c>todoservice/{Vault:Environment}</c>.</item>
    ///   <item><c>Vault:Environment</c> (<c>VAULT_ENVIRONMENT</c>) — defaults to <c>ASPNETCORE_ENVIRONMENT</c> lowercased.</item>
    ///   <item><c>Vault:Optional</c> (<c>VAULT_OPTIONAL</c>) — <c>true</c> to swallow connection errors. Defaults to <c>false</c>.</item>
    /// </list>
    /// </summary>
    public static IConfigurationBuilder AddVaultIfConfigured(
        this IConfigurationBuilder builder,
        string environmentName)
    {
        // Snapshot what's already been loaded (appsettings + env vars + ...)
        // so we can read Vault wiring from it without re-running everything.
        var snapshot = builder.Build();

        var address = snapshot["Vault:Address"] ?? snapshot["VAULT_ADDR"];
        var token   = snapshot["Vault:Token"]   ?? snapshot["VAULT_TOKEN"];

        if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(token))
        {
            // Vault not configured — skip silently. Local dev path.
            return builder;
        }

        var mount   = snapshot["Vault:Mount"] ?? snapshot["VAULT_MOUNT"] ?? "secret";
        var vaultEnv = snapshot["Vault:Environment"]
                       ?? snapshot["VAULT_ENVIRONMENT"]
                       ?? environmentName.ToLowerInvariant();
        var path    = snapshot["Vault:Path"]
                      ?? snapshot["VAULT_PATH"]
                      ?? $"todoservice/{vaultEnv}";

        var optionalRaw = snapshot["Vault:Optional"] ?? snapshot["VAULT_OPTIONAL"];
        var optional = bool.TryParse(optionalRaw, out var parsed) && parsed;

        builder.Add(new VaultConfigurationSource
        {
            Address  = address,
            Token    = token,
            Mount    = mount,
            Path     = path,
            Optional = optional,
        });

        return builder;
    }
}

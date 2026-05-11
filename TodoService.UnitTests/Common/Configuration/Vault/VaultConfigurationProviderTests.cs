using Microsoft.Extensions.Configuration;
using TodoService.Common.Configuration.Vault;

namespace TodoService.UnitTests.Common.Configuration.Vault;

/// <summary>
/// Live integration test against the Vault container started by
/// <c>docker-compose.local.yml</c>. Skipped automatically when Vault is not
/// reachable on the default local address, so this remains green on CI / on a
/// machine where Docker isn't running.
/// </summary>
public class VaultConfigurationProviderTests
{
    private const string VaultAddr = "http://127.0.0.1:8200";
    private const string VaultToken = "root";

    [Fact]
    public void Loads_ConnectionString_From_Vault_With_KeyNormalization()
    {
        if (!IsVaultReachable())
        {
            // Skip: local Vault not running. Treat as success so the suite
            // stays green outside a docker-compose.local.yml session.
            return;
        }

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Vault:Address"] = VaultAddr,
                ["Vault:Token"]   = VaultToken,
                ["Vault:Path"]    = "todoservice/dev",
            })
            .AddVaultIfConfigured("Development")
            .Build();

        // "ConnectionStrings__TodoDb" in Vault must surface as the standard
        // "ConnectionStrings:TodoDb" so GetConnectionString() finds it.
        var cs = config.GetConnectionString("TodoDb");

        Assert.False(string.IsNullOrWhiteSpace(cs));
        Assert.Contains("Database=todoservice_dev", cs);
    }

    [Fact]
    public void Optional_Source_Does_Not_Throw_When_Vault_Is_Unreachable()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Vault:Address"]  = "http://127.0.0.1:1", // closed port
                ["Vault:Token"]    = "root",
                ["Vault:Path"]     = "todoservice/dev",
                ["Vault:Optional"] = "true",
            })
            .AddVaultIfConfigured("Development")
            .Build();

        // No exception; just no Vault-sourced keys.
        Assert.Null(config.GetConnectionString("TodoDb"));
    }

    [Fact]
    public void Skips_Source_When_Vault_Address_Not_Configured()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TodoDb"] = "Host=local;Database=x",
            })
            .AddVaultIfConfigured("Development")
            .Build();

        Assert.Equal("Host=local;Database=x", config.GetConnectionString("TodoDb"));
    }

    private static bool IsVaultReachable()
    {
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            using var resp = http.GetAsync($"{VaultAddr}/v1/sys/health").GetAwaiter().GetResult();
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

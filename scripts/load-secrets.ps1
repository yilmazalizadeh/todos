<#
.SYNOPSIS
  Loads TodoService secrets from local dev Vault into the current PowerShell
  session as environment variables.

.DESCRIPTION
  Reads secret/todoservice/<Environment> from the Vault container started by
  docker-compose.local.yml and exports every key/value pair as a process-scoped
  environment variable. Keys in Vault use ASP.NET Core's
  double-underscore convention (e.g. ConnectionStrings__TodoDb), so the .NET
  configuration system picks them up automatically.

  Must be DOT-SOURCED so the variables stick in your shell:

      . .\scripts\load-secrets.ps1            # defaults to 'dev'
      . .\scripts\load-secrets.ps1 -Environment staging

  Then run the app from the same shell:

      dotnet run --project Services\TodoService\TodoService.csproj

.PARAMETER Environment
  Vault sub-path under secret/todoservice/. Defaults to 'dev'.

.PARAMETER VaultAddr
  Vault address. Defaults to http://127.0.0.1:8200.

.PARAMETER VaultToken
  Vault token. Defaults to 'root' (dev-mode token from docker-compose.local.yml).
#>
[CmdletBinding()]
param(
    [string]$Environment = 'dev',
    [string]$VaultAddr   = 'http://127.0.0.1:8200',
    [string]$VaultToken  = 'root'
)

$ErrorActionPreference = 'Stop'

$uri = "$VaultAddr/v1/secret/data/todoservice/$Environment"
Write-Host "Loading secrets from $uri ..." -ForegroundColor Cyan

try {
    $response = Invoke-RestMethod -Uri $uri -Headers @{ 'X-Vault-Token' = $VaultToken } -Method Get
} catch {
    Write-Error "Failed to read from Vault. Is docker-compose.local.yml running? ($_)"
    return
}

$data = $response.data.data
if ($null -eq $data) {
    Write-Error "No data found at secret/todoservice/$Environment"
    return
}

foreach ($prop in $data.PSObject.Properties) {
    [System.Environment]::SetEnvironmentVariable($prop.Name, $prop.Value, 'Process')
    Write-Host "  $($prop.Name) = (set, $($prop.Value.Length) chars)" -ForegroundColor Green
}

Write-Host "Done. Environment variables are set for this shell session." -ForegroundColor Cyan

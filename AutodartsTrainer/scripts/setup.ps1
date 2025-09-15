param(
    [string]$DotnetVersion = "7.0.410"
)

$ErrorActionPreference = "Stop"

function Install-DotNetSdk {
    param(
        [string]$Version
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnet -and (dotnet --list-sdks | Select-String $Version)) {
        Write-Host ".NET SDK $Version already installed."
        return
    }

    $installScript = Join-Path $env:TEMP "dotnet-install.ps1"
    Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $installScript
    & powershell -ExecutionPolicy Bypass -File $installScript -Version $Version -InstallDir "$env:ProgramFiles\dotnet"
}

Write-Host "Installing .NET SDK $DotnetVersion if required..."
Install-DotNetSdk -Version $DotnetVersion

Push-Location (Join-Path $PSScriptRoot "..")
try {
    Write-Host "Restoring AutodartsTrainer solution packages..."
    & dotnet restore AutodartsTrainer.sln
}
finally {
    Pop-Location
}

param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

Push-Location (Join-Path $PSScriptRoot "..")
try {
    Write-Host "Publishing AutodartsTrainer ($Configuration) for win-x64..."
    & dotnet publish AutodartsTrainer.sln -c $Configuration -r win-x64 --self-contained true /p:PublishSingleFile=true
}
finally {
    Pop-Location
}

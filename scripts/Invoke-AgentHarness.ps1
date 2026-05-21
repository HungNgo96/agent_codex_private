param(
    [ValidateSet("Quick", "Full")]
    [string]$Mode = "Quick",

    [switch]$RunSampleApi,

    [string]$OutputDir = ".harness-runs",

    [switch]$FailOnWarnings
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$resolvedOutputDir = if ([System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir
}
else {
    Join-Path $repoRoot $OutputDir
}

. (Join-Path $PSScriptRoot "harness\AgentHarness.Core.ps1")

$environment = @(Get-AgentHarnessEnvironment -RepoRoot $repoRoot)
$checks = @()
$checks += Invoke-AgentHarnessStaticChecks -RepoRoot $repoRoot -Mode $Mode
$checks += Invoke-AgentHarnessBuildChecks -RepoRoot $repoRoot

if ($Mode -eq "Full" -and $RunSampleApi) {
    $checks += Invoke-AgentHarnessSampleApiChecks -RepoRoot $repoRoot
}
elseif ($Mode -eq "Full") {
    $checks += New-AgentHarnessCheck `
        -Id "sample-api-endpoints" `
        -Name "Sample API endpoints" `
        -Status "Skipped" `
        -Severity "Info" `
        -Message "Endpoint checks were skipped because -RunSampleApi was not provided." `
        -Evidence @("Run with -Mode Full -RunSampleApi to boot and probe the sample API.")
}

if ($FailOnWarnings -and @($checks | Where-Object { $_.Status -eq "Warning" }).Count -gt 0) {
    foreach ($check in $checks | Where-Object { $_.Status -eq "Warning" }) {
        $check.Status = "Failed"
        $check.Severity = "Error"
        $check.Message = "FailOnWarnings: " + $check.Message
    }
}

if ($FailOnWarnings -and @($environment | Where-Object { $_.Status -eq "Warning" }).Count -gt 0) {
    foreach ($item in $environment | Where-Object { $_.Status -eq "Warning" }) {
        $item.Status = "Error"
        $item.Value = "FailOnWarnings: " + $item.Value
    }

    $checks += New-AgentHarnessCheck `
        -Id "environment-warnings" `
        -Name "Environment warnings" `
        -Status "Failed" `
        -Severity "Error" `
        -Message "FailOnWarnings: one or more environment warnings were reported." `
        -Evidence @($environment | Where-Object { $_.Status -eq "Error" } | ForEach-Object { "$($_.Name): $($_.Value)" })
}

$report = Write-AgentHarnessReport `
    -RepoRoot $repoRoot `
    -OutputDir $resolvedOutputDir `
    -Mode $Mode `
    -Checks $checks `
    -Environment $environment

$passed = @($checks | Where-Object { $_.Status -eq "Passed" }).Count
$failed = @($checks | Where-Object { $_.Status -eq "Failed" }).Count
$blocked = @($checks | Where-Object { $_.Status -eq "Blocked" }).Count
$warnings = @($checks | Where-Object { $_.Status -eq "Warning" }).Count
$skipped = @($checks | Where-Object { $_.Status -eq "Skipped" }).Count

Write-Host "Agent harness $($report.Status): $passed passed, $failed failed, $blocked blocked, $warnings warning, $skipped skipped."
Write-Host "Markdown report: $($report.MarkdownPath)"
Write-Host "JSON report: $($report.JsonPath)"

if ($blocked -gt 0) {
    exit 2
}

if ($failed -gt 0) {
    exit 1
}

exit 0

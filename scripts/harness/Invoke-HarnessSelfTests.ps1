param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "AgentHarness.Core.ps1")

function Assert-Equal {
    param(
        [object]$Actual,
        [object]$Expected,
        [string]$Message
    )

    if ($Actual -ne $Expected) {
        throw "$Message Expected '$Expected' but got '$Actual'."
    }
}

function Assert-True {
    param(
        [bool]$Condition,
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

$checks = Invoke-AgentHarnessStaticChecks -RepoRoot $RepoRoot -Mode Quick
$checkIds = @($checks | ForEach-Object { $_.Id })

Assert-True ($checkIds -contains "required-files") "Static checks should include required-files."
Assert-True ($checkIds -contains "markdown-sections") "Static checks should include markdown-sections."
Assert-True ($checkIds -contains "repo-local-links") "Static checks should include repo-local-links."
Assert-True ($checkIds -contains "handoff-contract") "Static checks should include handoff-contract."
Assert-True ($checkIds -contains "plan-contract") "Static checks should include plan-contract."

$sampleCheck = New-AgentHarnessCheck `
    -Id "example" `
    -Name "Example check" `
    -Status "Passed" `
    -Severity "Info" `
    -Message "Example passed" `
    -Evidence @("line one")

Assert-Equal $sampleCheck.Id "example" "Check id should be retained."
Assert-Equal $sampleCheck.Status "Passed" "Check status should be retained."
Assert-Equal $sampleCheck.Severity "Info" "Check severity should be retained."

$reportRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("agent-harness-selftest-" + [Guid]::NewGuid().ToString("N"))
try {
    $report = Write-AgentHarnessReport `
        -RepoRoot $RepoRoot `
        -OutputDir $reportRoot `
        -Mode "Quick" `
        -Checks @($sampleCheck) `
        -Environment @(@{ Name = "self-test"; Value = "true"; Status = "Info" })

    Assert-True (Test-Path $report.MarkdownPath) "Markdown report should be written."
    Assert-True (Test-Path $report.JsonPath) "JSON report should be written."

    $json = Get-Content -Raw $report.JsonPath | ConvertFrom-Json
    Assert-Equal $json.status "Passed" "Report status should be Passed when all checks pass."
    Assert-Equal @($json.checks).Count 1 "Report should include one check."
}
finally {
    if (Test-Path $reportRoot) {
        Remove-Item -LiteralPath $reportRoot -Recurse -Force
    }
}

Write-Host "Harness self-tests passed."

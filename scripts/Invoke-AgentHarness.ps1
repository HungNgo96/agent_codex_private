[CmdletBinding()]
param(
    [ValidateSet("Quick", "Full", "Status")]
    [string] $Mode = "Quick",
    [int] $Port = 5062,
    [string] $Configuration = "Debug",
    [int] $StartupTimeoutSeconds = 30
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$runId = Get-Date -Format "yyyyMMdd-HHmmss"
$runsRoot = Join-Path $repoRoot ".harness-runs"
$runDir = Join-Path $runsRoot $runId
$resultPath = Join-Path $runDir "result.json"
$apiProject = Join-Path $repoRoot "src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj"
$testProject = Join-Path $repoRoot "src\AgentTeams.SampleApi.Tests\AgentTeams.SampleApi.Tests.csproj"

function Write-StatusLine {
    param(
        [string] $Label,
        [object] $Value
    )

    Write-Host ("{0}: {1}" -f $Label, $Value)
}

function Save-HarnessResult {
    param(
        [hashtable] $Result,
        [string] $Status
    )

    $Result.status = $Status
    $Result.finishedAt = (Get-Date).ToUniversalTime().ToString("o")
    $Result | ConvertTo-Json -Depth 10 | Set-Content -Path $Result.resultPath -Encoding UTF8
}

function Invoke-LoggedCommand {
    param(
        [hashtable] $Result,
        [string] $Name,
        [string] $FilePath,
        [string[]] $Arguments
    )

    $logPath = Join-Path $Result.runDirectory "$Name.log"
    $stdoutPath = Join-Path $Result.runDirectory "$Name.stdout.tmp"
    $stderrPath = Join-Path $Result.runDirectory "$Name.stderr.tmp"
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

    $process = Start-Process `
        -FilePath $FilePath `
        -ArgumentList $Arguments `
        -WorkingDirectory $repoRoot `
        -RedirectStandardOutput $stdoutPath `
        -RedirectStandardError $stderrPath `
        -WindowStyle Hidden `
        -Wait `
        -PassThru

    $commandExitCode = $process.ExitCode
    $stopwatch.Stop()

    $stdout = if (Test-Path $stdoutPath) { Get-Content -Raw -Encoding UTF8 $stdoutPath } else { "" }
    $stderr = if (Test-Path $stderrPath) { Get-Content -Raw -Encoding UTF8 $stderrPath } else { "" }
    ($stdout + $stderr) | Set-Content -Path $logPath -Encoding UTF8
    Remove-Item -LiteralPath $stdoutPath, $stderrPath -Force -ErrorAction SilentlyContinue

    $Result.commands += [ordered]@{
        name = $Name
        command = "$FilePath $($Arguments -join ' ')"
        status = if ($commandExitCode -eq 0) { "Passed" } else { "Failed" }
        exitCode = $commandExitCode
        durationMs = $stopwatch.ElapsedMilliseconds
        logPath = $logPath
    }

    if ($commandExitCode -ne 0) {
        throw "$Name failed with exit code $commandExitCode. See $logPath."
    }
}

function Invoke-QuickHarness {
    New-Item -ItemType Directory -Path $runDir -Force | Out-Null

    $result = @{
        runId = $runId
        mode = "Quick"
        status = "Running"
        startedAt = (Get-Date).ToUniversalTime().ToString("o")
        finishedAt = $null
        runDirectory = $runDir
        resultPath = $resultPath
        commands = @()
        endpoints = @()
        errors = @()
    }

    try {
        Invoke-LoggedCommand -Result $result -Name "build" -FilePath "dotnet" -Arguments @(
            "build",
            $apiProject,
            "--configuration",
            $Configuration
        )

        Invoke-LoggedCommand -Result $result -Name "test" -FilePath "dotnet" -Arguments @(
            "test",
            $testProject,
            "--configuration",
            $Configuration
        )

        Invoke-LoggedCommand -Result $result -Name "diff-check" -FilePath "git" -Arguments @(
            "diff",
            "--check"
        )

        Save-HarnessResult -Result $result -Status "Passed"
        Write-Host "Quick harness passed. Evidence: $resultPath"
        return 0
    }
    catch {
        $result.errors += [ordered]@{
            message = $_.Exception.Message
            at = (Get-Date).ToUniversalTime().ToString("o")
        }
        Save-HarnessResult -Result $result -Status "Failed"
        Write-Error "Quick harness failed. Evidence: $resultPath. $($_.Exception.Message)"
        return 1
    }
}

function Invoke-FullHarness {
    $fullHarness = Join-Path $PSScriptRoot "run-dotnet-api-harness.ps1"
    & $fullHarness `
        -Port $Port `
        -Configuration $Configuration `
        -StartupTimeoutSeconds $StartupTimeoutSeconds

    return $LASTEXITCODE
}

function Show-HarnessStatus {
    if (-not (Test-Path $runsRoot)) {
        Write-Error "No harness runs found under $runsRoot."
        return 1
    }

    $latestResult = Get-ChildItem -Path $runsRoot -Recurse -Filter "result.json" |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if ($null -eq $latestResult) {
        Write-Error "No harness result.json files found under $runsRoot."
        return 1
    }

    $result = Get-Content -Raw -Encoding UTF8 $latestResult.FullName | ConvertFrom-Json

    Write-StatusLine -Label "Result" -Value $latestResult.FullName
    Write-StatusLine -Label "Run ID" -Value $result.runId
    if ($result.PSObject.Properties.Name -contains "mode") {
        Write-StatusLine -Label "Mode" -Value $result.mode
    }
    Write-StatusLine -Label "Status" -Value $result.status
    Write-StatusLine -Label "Started" -Value $result.startedAt
    Write-StatusLine -Label "Finished" -Value $result.finishedAt

    if ($null -ne $result.commands) {
        Write-Host ""
        Write-Host "Commands:"
        foreach ($command in $result.commands) {
            if ($command.PSObject.Properties.Name -contains "durationMs") {
                Write-Host ("- {0}: {1} ({2} ms)" -f $command.name, $command.status, $command.durationMs)
            }
            else {
                Write-Host ("- {0}: {1}" -f $command.name, $command.status)
            }
        }
    }

    if ($null -ne $result.endpoints -and $result.endpoints.Count -gt 0) {
        Write-Host ""
        Write-Host "Endpoints:"
        foreach ($endpoint in $result.endpoints) {
            Write-Host ("- {0} {1}: {2} HTTP {3} ({4} ms)" -f $endpoint.method, $endpoint.path, $endpoint.status, $endpoint.statusCode, $endpoint.durationMs)
        }
    }

    if ($null -ne $result.errors -and $result.errors.Count -gt 0) {
        Write-Host ""
        Write-Host "Errors:"
        foreach ($errorEntry in $result.errors) {
            Write-Host ("- {0}" -f $errorEntry.message)
        }
    }

    if ($result.status -eq "Passed") {
        return 0
    }

    return 1
}

switch ($Mode) {
    "Quick" { exit (Invoke-QuickHarness) }
    "Full" { exit (Invoke-FullHarness) }
    "Status" { exit (Show-HarnessStatus) }
}

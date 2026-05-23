[CmdletBinding()]
param(
    [int] $Port = 5062,
    [string] $Configuration = "Debug",
    [int] $StartupTimeoutSeconds = 30
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$apiProject = Join-Path $repoRoot "src\AgentTeams.SampleApi\AgentTeams.SampleApi.csproj"
$testProject = Join-Path $repoRoot "src\AgentTeams.SampleApi.Tests\AgentTeams.SampleApi.Tests.csproj"
$runId = Get-Date -Format "yyyyMMdd-HHmmss"
$runDir = Join-Path $repoRoot ".harness-runs\$runId"
$baseUrl = "http://127.0.0.1:$Port"
$resultPath = Join-Path $runDir "result.json"
$apiProcess = $null
$exitCode = 0
$createdEmployeeId = $null
$accessToken = $null

New-Item -ItemType Directory -Path $runDir -Force | Out-Null

$result = [ordered]@{
    runId = $runId
    status = "Running"
    startedAt = (Get-Date).ToUniversalTime().ToString("o")
    finishedAt = $null
    baseUrl = $baseUrl
    runDirectory = $runDir
    resultPath = $resultPath
    commands = @()
    endpoints = @()
    errors = @()
}

function Save-HarnessResult {
    param([string] $Status)

    $result.status = $Status
    $result.finishedAt = (Get-Date).ToUniversalTime().ToString("o")
    $result | ConvertTo-Json -Depth 10 | Set-Content -Path $resultPath -Encoding UTF8
}

function Invoke-DotNetStep {
    param(
        [string] $Name,
        [string[]] $Arguments
    )

    $logPath = Join-Path $runDir "$Name.log"
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $output = & dotnet @Arguments 2>&1
    $commandExitCode = $LASTEXITCODE
    $stopwatch.Stop()

    $output | Out-String | Set-Content -Path $logPath -Encoding UTF8

    $entry = [ordered]@{
        name = $Name
        command = "dotnet $($Arguments -join ' ')"
        status = if ($commandExitCode -eq 0) { "Passed" } else { "Failed" }
        exitCode = $commandExitCode
        durationMs = $stopwatch.ElapsedMilliseconds
        logPath = $logPath
    }
    $result.commands += $entry

    if ($commandExitCode -ne 0) {
        throw "$Name failed with exit code $commandExitCode. See $logPath."
    }
}

function Wait-ForApi {
    $deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
    $openApiUrl = "$baseUrl/openapi/v1.json"

    while ((Get-Date) -lt $deadline) {
        if ($apiProcess -ne $null -and $apiProcess.HasExited) {
            throw "API process exited before startup completed. See API logs in $runDir."
        }

        try {
            Invoke-RestMethod -Uri $openApiUrl -Method Get -TimeoutSec 2 | Out-Null
            return
        }
        catch {
            Start-Sleep -Milliseconds 500
        }
    }

    throw "API did not respond at $openApiUrl within $StartupTimeoutSeconds seconds."
}

function Invoke-EndpointProbe {
    param(
        [string] $Name,
        [string] $Method,
        [string] $Path,
        [int] $ExpectedStatusCode,
        [object] $Body = $null,
        [string] $BearerToken = $null,
        [scriptblock] $Validate = $null
    )

    $uri = "$baseUrl$Path"
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $statusCode = $null
    $responseContent = $null

    try {
        $request = @{
            Uri = $uri
            Method = $Method
            TimeoutSec = 10
            UseBasicParsing = $true
        }

        if ($null -ne $Body) {
            $request.ContentType = "application/json"
            $request.Body = ($Body | ConvertTo-Json -Depth 10)
        }

        if (-not [string]::IsNullOrWhiteSpace($BearerToken)) {
            $request.Headers = @{
                Authorization = "Bearer $BearerToken"
            }
        }

        $response = Invoke-WebRequest @request
        $statusCode = [int] $response.StatusCode
        $responseContent = $response.Content

        if ($statusCode -ne $ExpectedStatusCode) {
            throw "Expected HTTP $ExpectedStatusCode but received HTTP $statusCode."
        }

        $json = $null
        if (-not [string]::IsNullOrWhiteSpace($responseContent)) {
            $json = $responseContent | ConvertFrom-Json
        }

        if ($null -ne $Validate) {
            & $Validate $json
        }

        $stopwatch.Stop()
        $result.endpoints += [ordered]@{
            name = $Name
            method = $Method
            path = $Path
            status = "Passed"
            statusCode = $statusCode
            durationMs = $stopwatch.ElapsedMilliseconds
        }
    }
    catch {
        $stopwatch.Stop()
        $result.endpoints += [ordered]@{
            name = $Name
            method = $Method
            path = $Path
            status = "Failed"
            statusCode = $statusCode
            durationMs = $stopwatch.ElapsedMilliseconds
            error = $_.Exception.Message
        }
        throw "$Name probe failed: $($_.Exception.Message)"
    }
}

$previousEnvironment = @{
    ASPNETCORE_ENVIRONMENT = $env:ASPNETCORE_ENVIRONMENT
    ASPNETCORE_URLS = $env:ASPNETCORE_URLS
    Auth__Issuer = $env:Auth__Issuer
    Auth__Audience = $env:Auth__Audience
    Auth__SigningKey = $env:Auth__SigningKey
    Database__Provider = $env:Database__Provider
    ConnectionStrings__EmployeeDatabase = $env:ConnectionStrings__EmployeeDatabase
}

try {
    Invoke-DotNetStep -Name "build" -Arguments @(
        "build",
        $apiProject,
        "--configuration",
        $Configuration
    )

    Invoke-DotNetStep -Name "test" -Arguments @(
        "test",
        $testProject,
        "--configuration",
        $Configuration
    )

    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:ASPNETCORE_URLS = $baseUrl
    $env:Auth__Issuer = "AgentTeams.Harness"
    $env:Auth__Audience = "AgentTeams.SampleApi"
    $env:Auth__SigningKey = "agent-teams-harness-signing-key-must-be-long-enough"
    $env:Database__Provider = "Sqlite"
    $env:ConnectionStrings__EmployeeDatabase = "Data Source=$(Join-Path $runDir 'employees.db')"

    $apiStdOut = Join-Path $runDir "api.stdout.log"
    $apiStdErr = Join-Path $runDir "api.stderr.log"
    $apiProcess = Start-Process `
        -FilePath "dotnet" `
        -ArgumentList @(
            "run",
            "--project",
            $apiProject,
            "--configuration",
            $Configuration,
            "--no-build",
            "--urls",
            $baseUrl
        ) `
        -WorkingDirectory $repoRoot `
        -RedirectStandardOutput $apiStdOut `
        -RedirectStandardError $apiStdErr `
        -WindowStyle Hidden `
        -PassThru

    $result.commands += [ordered]@{
        name = "api-start"
        command = "dotnet run --project $apiProject --configuration $Configuration --no-build --urls $baseUrl"
        status = "Started"
        processId = $apiProcess.Id
        stdoutLogPath = $apiStdOut
        stderrLogPath = $apiStdErr
    }

    Wait-ForApi

    $devTokenRequest = [ordered]@{
        subject = "harness"
        name = "Harness"
        scopes = @("employees.write")
    }

    Invoke-EndpointProbe -Name "dev-token-create" -Method "POST" -Path "/api/v1/auth/dev-token" -ExpectedStatusCode 200 -Body $devTokenRequest -Validate {
        param($json)
        if ([string]::IsNullOrWhiteSpace($json.accessToken)) {
            throw "Expected access token."
        }
        if ($json.tokenType -ne "Bearer") {
            throw "Expected Bearer token type."
        }
        $script:accessToken = $json.accessToken
    }

    Invoke-EndpointProbe -Name "products-list" -Method "GET" -Path "/api/v1/products" -ExpectedStatusCode 200 -Validate {
        param($json)
        if ($json.Count -lt 1) {
            throw "Expected at least one product."
        }
    }

    Invoke-EndpointProbe -Name "agent-workflows-list" -Method "GET" -Path "/api/v1/agent-workflows" -ExpectedStatusCode 200 -Validate {
        param($json)
        if ($json.Count -lt 4) {
            throw "Expected at least four workflow steps."
        }
    }

    Invoke-EndpointProbe -Name "harness-runs-list" -Method "GET" -Path "/api/v1/harness-runs" -ExpectedStatusCode 200 -Validate {
        param($json)
        if ($json.Count -lt 1) {
            throw "Expected at least one sample harness run."
        }
    }

    Invoke-EndpointProbe -Name "employees-list" -Method "GET" -Path "/api/v1/employees" -ExpectedStatusCode 200 -Validate {
        param($json)
        if ($null -eq $json.items) {
            throw "Expected employee page response with items."
        }
    }

    Invoke-EndpointProbe -Name "employee-storage" -Method "GET" -Path "/api/v1/employees/storage" -ExpectedStatusCode 200 -Validate {
        param($json)
        if ($json.provider -ne "sqlite") {
            throw "Expected sqlite storage provider."
        }
        if (-not $json.autoMigrationsEnabled) {
            throw "Expected automatic migrations to be enabled."
        }
    }

    $basicInfoRequest = [ordered]@{
        employeeCode = "HARNESS-$runId"
        fullName = "Harness Employee"
        email = "harness-$runId@example.com"
    }

    Invoke-EndpointProbe -Name "employee-basic-info-create" -Method "POST" -Path "/api/v1/employees/basic-info" -ExpectedStatusCode 201 -Body $basicInfoRequest -BearerToken $accessToken -Validate {
        param($json)
        if ([string]::IsNullOrWhiteSpace($json.id)) {
            throw "Expected created employee id."
        }
        if ($json.department -ne "General") {
            throw "Expected default department General."
        }
        if ($json.jobTitle -ne "Employee") {
            throw "Expected default job title Employee."
        }
        $script:createdEmployeeId = $json.id
    }

    Invoke-EndpointProbe -Name "employee-basic-info-readback" -Method "GET" -Path "/api/v1/employees/$createdEmployeeId" -ExpectedStatusCode 200 -Validate {
        param($json)
        if ($json.employeeCode -ne "HARNESS-$runId") {
            throw "Read-back employee code did not match created employee."
        }
    }

    Save-HarnessResult -Status "Passed"
    Write-Host "Harness passed. Evidence: $resultPath"
}
catch {
    $exitCode = 1
    $result.errors += [ordered]@{
        message = $_.Exception.Message
        at = (Get-Date).ToUniversalTime().ToString("o")
    }
    Save-HarnessResult -Status "Failed"
    Write-Error "Harness failed. Evidence: $resultPath. $($_.Exception.Message)"
}
finally {
    if ($apiProcess -ne $null -and -not $apiProcess.HasExited) {
        Stop-Process -Id $apiProcess.Id -Force
        $apiProcess.WaitForExit()
    }

    $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment.ASPNETCORE_ENVIRONMENT
    $env:ASPNETCORE_URLS = $previousEnvironment.ASPNETCORE_URLS
    $env:Auth__Issuer = $previousEnvironment.Auth__Issuer
    $env:Auth__Audience = $previousEnvironment.Auth__Audience
    $env:Auth__SigningKey = $previousEnvironment.Auth__SigningKey
    $env:Database__Provider = $previousEnvironment.Database__Provider
    $env:ConnectionStrings__EmployeeDatabase = $previousEnvironment.ConnectionStrings__EmployeeDatabase
}

exit $exitCode

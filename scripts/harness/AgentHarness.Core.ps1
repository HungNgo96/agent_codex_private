$Script:AgentHarnessStatuses = @("Passed", "Failed", "Warning", "Skipped", "Blocked")
$Script:AgentHarnessSeverities = @("Info", "Warning", "Error")

function New-AgentHarnessCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Id,
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][ValidateSet("Passed", "Failed", "Warning", "Skipped", "Blocked")][string]$Status,
        [Parameter(Mandatory = $true)][ValidateSet("Info", "Warning", "Error")][string]$Severity,
        [Parameter(Mandatory = $true)][string]$Message,
        [string[]]$Evidence = @()
    )

    [pscustomobject][ordered]@{
        Id       = $Id
        Name     = $Name
        Status   = $Status
        Severity = $Severity
        Message  = $Message
        Evidence = @($Evidence)
    }
}

function New-AgentHarnessEnvironmentItem {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Value,
        [ValidateSet("Info", "Warning", "Error")][string]$Status = "Info"
    )

    [pscustomobject][ordered]@{
        Name   = $Name
        Value  = $Value
        Status = $Status
    }
}

function Invoke-AgentHarnessCommand {
    param(
        [Parameter(Mandatory = $true)][string]$FileName,
        [string]$Arguments = "",
        [string]$WorkingDirectory = (Get-Location).Path,
        [int]$TimeoutSeconds = 120
    )

    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = $FileName
    $startInfo.Arguments = $Arguments
    $startInfo.WorkingDirectory = $WorkingDirectory
    $startInfo.UseShellExecute = $false
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.CreateNoWindow = $true

    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $startInfo

    try {
        [void]$process.Start()
    }
    catch {
        return [pscustomobject][ordered]@{
            ExitCode = 127
            Output   = ""
            Error    = $_.Exception.Message
            TimedOut = $false
        }
    }

    $completed = $process.WaitForExit($TimeoutSeconds * 1000)
    if (-not $completed) {
        try {
            $process.Kill()
        }
        catch {
        }

        return [pscustomobject][ordered]@{
            ExitCode = 124
            Output   = $process.StandardOutput.ReadToEnd()
            Error    = "Timed out after $TimeoutSeconds seconds."
            TimedOut = $true
        }
    }

    [pscustomobject][ordered]@{
        ExitCode = $process.ExitCode
        Output   = $process.StandardOutput.ReadToEnd()
        Error    = $process.StandardError.ReadToEnd()
        TimedOut = $false
    }
}

function Test-AgentHarnessMarkdownSections {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string[]]$Sections
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return @("Missing file: $Path")
    }

    $content = Get-Content -Raw -LiteralPath $Path
    $missing = New-Object System.Collections.Generic.List[string]

    foreach ($section in $Sections) {
        $pattern = "(?m)^#{1,6}\s+" + [regex]::Escape($section) + "\s*$"
        if ($content -notmatch $pattern) {
            [void]$missing.Add($section)
        }
    }

    @($missing)
}

function Test-AgentHarnessRequiredFiles {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $required = @(
        "AGENTS.md",
        "ARCHITECTURE.md",
        "README.md",
        ".codex/agents/coder.toml",
        ".codex/agents/hermes.toml",
        ".codex/agents/leader.toml",
        ".codex/agents/reviewer.toml",
        ".claude/agents/coder.md",
        ".claude/agents/leader.md",
        ".claude/agents/reviewer.md",
        ".cursor/rules/agent-team-operating-contract.mdc",
        ".cursor/rules/agent-team-subagent-flow.mdc",
        "prompts/lead-agent.md",
        "prompts/worker-agent.md",
        "templates/task-brief.md",
        "templates/handoff-note.md",
        "templates/implementation-plan.md",
        "workflows/feature-work.md",
        "workflows/debugging.md",
        "workflows/code-review.md",
        "workflows/research.md",
        "teams/full-stack.md",
        "teams/debugging.md",
        "teams/research-review.md",
        "src/AgentTeams.SampleApi/AgentTeams.SampleApi.csproj"
    )

    $missing = @()
    foreach ($relativePath in $required) {
        $fullPath = Join-Path $RepoRoot $relativePath
        if (-not (Test-Path -LiteralPath $fullPath)) {
            $missing += $relativePath
        }
    }

    if ($missing.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "required-files" `
            -Name "Required files exist" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "Required repository files are missing." `
            -Evidence $missing
    }

    New-AgentHarnessCheck `
        -Id "required-files" `
        -Name "Required files exist" `
        -Status "Passed" `
        -Severity "Info" `
        -Message "All required repository files are present." `
        -Evidence @("$($required.Count) files checked.")
}

function Test-AgentHarnessKnowledgeStoreLayout {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $requiredPaths = @(
        "AGENTS.md",
        "ARCHITECTURE.md",
        "docs/design-docs",
        "docs/design-docs/index.md",
        "docs/design-docs/core-beliefs.md",
        "docs/exec-plans",
        "docs/exec-plans/active",
        "docs/exec-plans/active/index.md",
        "docs/exec-plans/completed",
        "docs/exec-plans/completed/index.md",
        "docs/exec-plans/tech-debt-tracker.md",
        "docs/generated",
        "docs/generated/db-schema.md",
        "docs/product-specs",
        "docs/product-specs/index.md",
        "docs/product-specs/new-user-onboarding.md",
        "docs/references",
        "docs/references/design-system-reference-llms.txt",
        "docs/references/nixpacks-llms.txt",
        "docs/references/uv-llms.txt",
        "docs/DESIGN.md",
        "docs/FRONTEND.md",
        "docs/PLANS.md",
        "docs/PRODUCT_SENSE.md",
        "docs/QUALITY_SCORE.md",
        "docs/RELIABILITY.md",
        "docs/SECURITY.md"
    )

    $missing = @()
    foreach ($relativePath in $requiredPaths) {
        $fullPath = Join-Path $RepoRoot $relativePath
        if (-not (Test-Path -LiteralPath $fullPath)) {
            $missing += $relativePath
        }
    }

    if ($missing.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "knowledge-store-layout" `
            -Name "In-repository knowledge store layout" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "Required knowledge-store paths are missing." `
            -Evidence $missing
    }

    New-AgentHarnessCheck `
        -Id "knowledge-store-layout" `
        -Name "In-repository knowledge store layout" `
        -Status "Passed" `
        -Severity "Info" `
        -Message "The in-repository knowledge store layout is present." `
        -Evidence @("$($requiredPaths.Count) paths checked.")
}

function Test-AgentHarnessSectionContracts {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $contracts = @(
        @{ Path = "AGENTS.md"; Sections = @("Shared Rules", "Lead Agent Responsibilities", "Worker Agent Responsibilities", "Delegation Rules", "Handoff Requirements", "Completion Standard") },
        @{ Path = "prompts/lead-agent.md"; Sections = @("Responsibilities", "Subagent Workflow", "Task Brief", "Completion Standard") },
        @{ Path = "prompts/worker-agent.md"; Sections = @("Responsibilities", "Handoff") },
        @{ Path = "templates/task-brief.md"; Sections = @("Goal", "Non-Goals", "Context", "Constraints", "Success Criteria", "Delegation Decision", "Team", "Ownership", "Verification Plan") },
        @{ Path = "templates/handoff-note.md"; Sections = @("Assigned Scope", "Completed Work", "Files Changed", "Evidence Gathered", "Commands Run", "Verification", "Risks and Assumptions", "Integration Notes", "Suggested Next Step") },
        @{ Path = "templates/implementation-plan.md"; Sections = @("Goal", "Files", "Tasks", "Verification", "Handoff") }
    )

    $missingEvidence = @()
    foreach ($contract in $contracts) {
        $fullPath = Join-Path $RepoRoot $contract.Path
        $missing = Test-AgentHarnessMarkdownSections -Path $fullPath -Sections $contract.Sections
        foreach ($section in $missing) {
            $missingEvidence += "$($contract.Path): missing section '$section'"
        }
    }

    if ($missingEvidence.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "markdown-sections" `
            -Name "Markdown section contracts" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "One or more required Markdown sections are missing." `
            -Evidence $missingEvidence
    }

    New-AgentHarnessCheck `
        -Id "markdown-sections" `
        -Name "Markdown section contracts" `
        -Status "Passed" `
        -Severity "Info" `
        -Message "Required Markdown sections are present." `
        -Evidence @("$($contracts.Count) files checked.")
}

function Test-AgentHarnessRepoLocalLinks {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $includedRoots = @(
        "docs",
        "examples",
        "prompts",
        "scripts",
        "teams",
        "templates",
        "workflows"
    )

    $markdownFiles = @()
    foreach ($rootName in $includedRoots) {
        $rootPath = Join-Path $RepoRoot $rootName
        if (Test-Path -LiteralPath $rootPath) {
            $markdownFiles += Get-ChildItem -LiteralPath $rootPath -Recurse -File -Filter "*.md"
        }
    }

    foreach ($rootFile in @("AGENTS.md", "README.md", "CLAUDE.md", "overview-agent-team-flow.md")) {
        $path = Join-Path $RepoRoot $rootFile
        if (Test-Path -LiteralPath $path) {
            $markdownFiles += Get-Item -LiteralPath $path
        }
    }

    $broken = @()
    $checked = 0
    $linkPattern = "\[[^\]]+\]\(([^)]+)\)"

    foreach ($file in $markdownFiles) {
        $content = Get-Content -Raw -LiteralPath $file.FullName
        foreach ($match in [regex]::Matches($content, $linkPattern)) {
            $target = $match.Groups[1].Value.Trim()
            if ($target -match "^(https?:|mailto:|#)" -or $target.Length -eq 0) {
                continue
            }

            $target = $target.Split(" ")[0].Trim("<", ">")
            $targetPath = $target.Split("#")[0]
            if ($targetPath.Length -eq 0) {
                continue
            }

            $checked++
            $resolved = Join-Path $file.DirectoryName $targetPath
            if (-not (Test-Path -LiteralPath $resolved)) {
                $relativeFile = Resolve-AgentHarnessRelativePath -RepoRoot $RepoRoot -Path $file.FullName
                $broken += "$relativeFile -> $target"
            }
        }
    }

    if ($broken.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "repo-local-links" `
            -Name "Repository-local Markdown links" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "Broken repository-local Markdown links were found." `
            -Evidence $broken
    }

    New-AgentHarnessCheck `
        -Id "repo-local-links" `
        -Name "Repository-local Markdown links" `
        -Status "Passed" `
        -Severity "Info" `
        -Message "Repository-local Markdown links resolve." `
        -Evidence @("$checked local links checked.")
}

function Test-AgentHarnessHandoffContract {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $path = Join-Path $RepoRoot "templates/handoff-note.md"
    $content = Get-Content -Raw -LiteralPath $path
    $requiredTerms = @(
        "Assigned Scope",
        "Completed Work",
        "Files Changed",
        "Evidence Gathered",
        "Commands Run",
        "Verification",
        "Risks and Assumptions",
        "Integration Notes",
        "Suggested Next Step"
    )

    $missing = @()
    foreach ($term in $requiredTerms) {
        if ($content -notmatch [regex]::Escape($term)) {
            $missing += $term
        }
    }

    if ($missing.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "handoff-contract" `
            -Name "Handoff contract" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "The handoff template is missing required contract terms." `
            -Evidence $missing
    }

    New-AgentHarnessCheck `
        -Id "handoff-contract" `
        -Name "Handoff contract" `
        -Status "Passed" `
        -Severity "Info" `
        -Message "The handoff template includes the required evidence fields." `
        -Evidence @("templates/handoff-note.md")
}

function Test-AgentHarnessPlanContract {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $planRoots = @(
        @{ Path = "docs/exec-plans/active"; Required = $true },
        @{ Path = "docs/exec-plans/completed"; Required = $true },
        @{ Path = "docs/superpowers/plans"; Required = $false }
    )

    $missingRoots = @()
    $plans = @()
    foreach ($planRoot in $planRoots) {
        $fullPath = Join-Path $RepoRoot $planRoot.Path
        if (-not (Test-Path -LiteralPath $fullPath)) {
            if ($planRoot.Required) {
                $missingRoots += $planRoot.Path
            }
            continue
        }

        $plans += @(Get-ChildItem -LiteralPath $fullPath -File -Filter "*.md" | Where-Object { $_.Name -ne "index.md" })
    }

    if ($missingRoots.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "plan-contract" `
            -Name "Implementation plan contract" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "One or more primary execution plan directories are missing." `
            -Evidence $missingRoots
    }

    if ($plans.Count -eq 0) {
        return New-AgentHarnessCheck `
            -Id "plan-contract" `
            -Name "Implementation plan contract" `
            -Status "Warning" `
            -Severity "Warning" `
            -Message "No implementation plans were found." `
            -Evidence @("docs/exec-plans/active", "docs/exec-plans/completed", "docs/superpowers/plans")
    }

    $requiredTerms = @("Goal:", "Architecture:", "Tech Stack:", "- [ ]", "Verification")
    $missingEvidence = @()

    foreach ($plan in $plans) {
        $content = Get-Content -Raw -LiteralPath $plan.FullName
        foreach ($term in $requiredTerms) {
            if ($content -notmatch [regex]::Escape($term)) {
                $relative = Resolve-AgentHarnessRelativePath -RepoRoot $RepoRoot -Path $plan.FullName
                $missingEvidence += "${relative}: missing '$term'"
            }
        }
    }

    if ($missingEvidence.Count -gt 0) {
        return New-AgentHarnessCheck `
            -Id "plan-contract" `
            -Name "Implementation plan contract" `
            -Status "Failed" `
            -Severity "Error" `
            -Message "One or more implementation plans are missing required planning fields." `
            -Evidence $missingEvidence
    }

    New-AgentHarnessCheck `
        -Id "plan-contract" `
        -Name "Implementation plan contract" `
        -Status "Passed" `
        -Severity "Info" `
        -Message "Implementation plans include required planning fields." `
        -Evidence @("$($plans.Count) plan files checked across primary and legacy plan directories.")
}

function Resolve-AgentHarnessRelativePath {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [Parameter(Mandatory = $true)][string]$Path
    )

    $root = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd("\", "/")
    $full = [System.IO.Path]::GetFullPath($Path)
    if ($full.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $full.Substring($root.Length).TrimStart("\", "/")
    }

    $Path
}

function Invoke-AgentHarnessStaticChecks {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [ValidateSet("Quick", "Full")][string]$Mode = "Quick"
    )

    @(
        Test-AgentHarnessRequiredFiles -RepoRoot $RepoRoot
        Test-AgentHarnessKnowledgeStoreLayout -RepoRoot $RepoRoot
        Test-AgentHarnessSectionContracts -RepoRoot $RepoRoot
        Test-AgentHarnessRepoLocalLinks -RepoRoot $RepoRoot
        Test-AgentHarnessHandoffContract -RepoRoot $RepoRoot
        Test-AgentHarnessPlanContract -RepoRoot $RepoRoot
    )
}

function Get-AgentHarnessEnvironment {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $items = @()
    $items += New-AgentHarnessEnvironmentItem -Name "repoRoot" -Value $RepoRoot
    $items += New-AgentHarnessEnvironmentItem -Name "powershell" -Value $PSVersionTable.PSVersion.ToString()

    $dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnetCommand) {
        $dotnetVersion = Invoke-AgentHarnessCommand -FileName "dotnet" -Arguments "--version" -WorkingDirectory $RepoRoot -TimeoutSeconds 30
        if ($dotnetVersion.ExitCode -eq 0) {
            $items += New-AgentHarnessEnvironmentItem -Name "dotnet" -Value $dotnetVersion.Output.Trim()
        }
        else {
            $items += New-AgentHarnessEnvironmentItem -Name "dotnet" -Value $dotnetVersion.Error.Trim() -Status "Warning"
        }
    }
    else {
        $items += New-AgentHarnessEnvironmentItem -Name "dotnet" -Value "dotnet command not found" -Status "Error"
    }

    $gitCommand = Get-Command git -ErrorAction SilentlyContinue
    if ($gitCommand) {
        $gitStatus = Invoke-AgentHarnessCommand -FileName "git" -Arguments "status --short" -WorkingDirectory $RepoRoot -TimeoutSeconds 30
        if ($gitStatus.ExitCode -eq 0) {
            $items += New-AgentHarnessEnvironmentItem -Name "gitSafeDirectory" -Value "git status is available"
        }
        else {
            $message = ($gitStatus.Error + $gitStatus.Output).Trim()
            if ($message.Length -gt 220) {
                $message = $message.Substring(0, 220) + "..."
            }
            $items += New-AgentHarnessEnvironmentItem -Name "gitSafeDirectory" -Value $message -Status "Warning"
        }
    }
    else {
        $items += New-AgentHarnessEnvironmentItem -Name "git" -Value "git command not found" -Status "Warning"
    }

    $items
}

function Invoke-AgentHarnessBuildChecks {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    $project = Join-Path $RepoRoot "src/AgentTeams.SampleApi/AgentTeams.SampleApi.csproj"
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        return New-AgentHarnessCheck `
            -Id "sample-api-build" `
            -Name "Sample API build" `
            -Status "Blocked" `
            -Severity "Error" `
            -Message "dotnet is not available, so the sample API cannot be built." `
            -Evidence @("Install the .NET SDK required by src/AgentTeams.SampleApi.")
    }

    $result = Invoke-AgentHarnessCommand -FileName "dotnet" -Arguments "build `"$project`" --nologo" -WorkingDirectory $RepoRoot -TimeoutSeconds 180
    if ($result.ExitCode -eq 0) {
        return New-AgentHarnessCheck `
            -Id "sample-api-build" `
            -Name "Sample API build" `
            -Status "Passed" `
            -Severity "Info" `
            -Message "The sample API builds successfully." `
            -Evidence @(($result.Output -split "`r?`n" | Where-Object { $_.Trim().Length -gt 0 } | Select-Object -Last 5))
    }

    New-AgentHarnessCheck `
        -Id "sample-api-build" `
        -Name "Sample API build" `
        -Status "Failed" `
        -Severity "Error" `
        -Message "The sample API build failed." `
        -Evidence @(($result.Output + "`n" + $result.Error) -split "`r?`n" | Where-Object { $_.Trim().Length -gt 0 } | Select-Object -Last 12)
}

function Get-AgentHarnessFreePort {
    $listener = New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Parse("127.0.0.1"), 0)
    $listener.Start()
    try {
        return $listener.LocalEndpoint.Port
    }
    finally {
        $listener.Stop()
    }
}

function Invoke-AgentHarnessSampleApiChecks {
    param([Parameter(Mandatory = $true)][string]$RepoRoot)

    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        return New-AgentHarnessCheck `
            -Id "sample-api-endpoints" `
            -Name "Sample API endpoints" `
            -Status "Blocked" `
            -Severity "Error" `
            -Message "dotnet is not available, so endpoint checks cannot run." `
            -Evidence @("Install the .NET SDK required by src/AgentTeams.SampleApi.")
    }

    $port = Get-AgentHarnessFreePort
    $baseUrl = "http://127.0.0.1:$port"
    $project = Join-Path $RepoRoot "src/AgentTeams.SampleApi/AgentTeams.SampleApi.csproj"
    $runDir = Join-Path ([System.IO.Path]::GetTempPath()) ("agent-harness-api-" + [Guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Path $runDir -Force | Out-Null
    $stdout = Join-Path $runDir "stdout.log"
    $stderr = Join-Path $runDir "stderr.log"
    $employeeDatabasePath = Join-Path $runDir "employees.db"
    $previousDatabaseProvider = $env:Database__Provider
    $previousEmployeeConnectionString = $env:ConnectionStrings__EmployeeDatabase

    $process = $null
    try {
        $env:Database__Provider = "Sqlite"
        $env:ConnectionStrings__EmployeeDatabase = "Data Source=$employeeDatabasePath"

        $process = Start-Process `
            -FilePath "dotnet" `
            -ArgumentList @("run", "--project", $project, "--urls", $baseUrl) `
            -WorkingDirectory $RepoRoot `
            -RedirectStandardOutput $stdout `
            -RedirectStandardError $stderr `
            -PassThru `
            -WindowStyle Hidden

        $ready = $false
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            Start-Sleep -Milliseconds 500
            if ($process.HasExited) {
                break
            }

            try {
                Invoke-RestMethod -Uri "$baseUrl/api/v1/products" -TimeoutSec 2 | Out-Null
                $ready = $true
                break
            }
            catch {
            }
        }

        if (-not $ready) {
            $evidence = @("Base URL: $baseUrl")
            if (Test-Path $stdout) {
                $evidence += (Get-Content -LiteralPath $stdout -Tail 8)
            }
            if (Test-Path $stderr) {
                $evidence += (Get-Content -LiteralPath $stderr -Tail 8)
            }

            return New-AgentHarnessCheck `
                -Id "sample-api-endpoints" `
                -Name "Sample API endpoints" `
                -Status "Failed" `
                -Severity "Error" `
                -Message "The sample API did not become ready for endpoint checks." `
                -Evidence $evidence
        }

        try {
            $products = Invoke-RestMethod -Uri "$baseUrl/api/v1/products" -TimeoutSec 5
            $product = Invoke-RestMethod -Uri "$baseUrl/api/v1/products/1" -TimeoutSec 5
            $workflows = Invoke-RestMethod -Uri "$baseUrl/api/v1/agent-workflows" -TimeoutSec 5
            $harnessRuns = Invoke-RestMethod -Uri "$baseUrl/api/v1/harness-runs" -TimeoutSec 5
            $harnessRun = Invoke-RestMethod -Uri "$baseUrl/api/v1/harness-runs/20260521-100730" -TimeoutSec 5
            $employeeStorage = Invoke-RestMethod -Uri "$baseUrl/api/v1/employees/storage" -TimeoutSec 5
            $createEmployeeBody = @{
                employeeCode = "HARNESS-001"
                fullName = "Harness Employee"
                email = "harness.employee@example.com"
                department = "Quality"
                jobTitle = "Harness Tester"
            } | ConvertTo-Json
            $createdEmployee = Invoke-RestMethod `
                -Method Post `
                -Uri "$baseUrl/api/v1/employees" `
                -ContentType "application/json" `
                -Body $createEmployeeBody `
                -TimeoutSec 5
            $employee = Invoke-RestMethod -Uri "$baseUrl/api/v1/employees/$($createdEmployee.id)" -TimeoutSec 5
            $openApi = Invoke-RestMethod -Uri "$baseUrl/openapi/v1.json" -TimeoutSec 5
        }
        catch {
            return New-AgentHarnessCheck `
                -Id "sample-api-endpoints" `
                -Name "Sample API endpoints" `
                -Status "Failed" `
                -Severity "Error" `
                -Message "The sample API endpoint probe failed." `
                -Evidence @($_.Exception.Message)
        }

        if (@($products).Count -lt 1 -or $product.id -ne 1 -or -not $openApi.openapi) {
            return New-AgentHarnessCheck `
                -Id "sample-api-endpoints" `
                -Name "Sample API endpoints" `
                -Status "Failed" `
                -Severity "Error" `
                -Message "The sample API returned unexpected endpoint data." `
                -Evidence @("Products count: $(@($products).Count)", "Product 1 id: $($product.id)", "OpenAPI version: $($openApi.openapi)")
        }

        if (@($workflows).Count -lt 3 -or @($harnessRuns).Count -lt 1 -or $harnessRun.id -ne "20260521-100730") {
            return New-AgentHarnessCheck `
                -Id "sample-api-endpoints" `
                -Name "Sample API endpoints" `
                -Status "Failed" `
                -Severity "Error" `
                -Message "The sample API returned unexpected harness-flow data." `
                -Evidence @("Workflow count: $(@($workflows).Count)", "Harness run count: $(@($harnessRuns).Count)", "Harness run id: $($harnessRun.id)")
        }

        if ($employeeStorage.provider -ne "sqlite" -or -not $employeeStorage.postgreSqlSupported -or $employee.employeeCode -ne "HARNESS-001") {
            return New-AgentHarnessCheck `
                -Id "sample-api-endpoints" `
                -Name "Sample API endpoints" `
                -Status "Failed" `
                -Severity "Error" `
                -Message "The sample API returned unexpected employee API data." `
                -Evidence @("Employee provider: $($employeeStorage.provider)", "PostgreSQL supported: $($employeeStorage.postgreSqlSupported)", "Employee code: $($employee.employeeCode)")
        }

        New-AgentHarnessCheck `
            -Id "sample-api-endpoints" `
            -Name "Sample API endpoints" `
            -Status "Passed" `
            -Severity "Info" `
            -Message "The sample API responded to product, OpenAPI, agent workflow, harness run, and employee probes." `
            -Evidence @(
                "$baseUrl/api/v1/products",
                "$baseUrl/api/v1/products/1",
                "$baseUrl/api/v1/agent-workflows",
                "$baseUrl/api/v1/harness-runs",
                "$baseUrl/api/v1/harness-runs/20260521-100730",
                "$baseUrl/api/v1/employees/storage",
                "$baseUrl/api/v1/employees",
                "$baseUrl/openapi/v1.json"
            )
    }
    finally {
        $env:Database__Provider = $previousDatabaseProvider
        $env:ConnectionStrings__EmployeeDatabase = $previousEmployeeConnectionString

        if ($process -and -not $process.HasExited) {
            Stop-Process -Id $process.Id -Force
            try {
                $process.WaitForExit(5000) | Out-Null
            }
            catch {
            }
        }

        if ($process) {
            try {
                $process.Dispose()
            }
            catch {
            }
        }

        if (Test-Path $runDir) {
            try {
                Remove-Item -LiteralPath $runDir -Recurse -Force
            }
            catch {
            }
        }
    }
}

function Write-AgentHarnessReport {
    param(
        [Parameter(Mandatory = $true)][string]$RepoRoot,
        [Parameter(Mandatory = $true)][string]$OutputDir,
        [ValidateSet("Quick", "Full")][string]$Mode = "Quick",
        [Parameter(Mandatory = $true)][object[]]$Checks,
        [Parameter(Mandatory = $true)][object[]]$Environment
    )

    $runId = Get-Date -Format "yyyyMMdd-HHmmss"
    $runDir = Join-Path $OutputDir $runId
    New-Item -ItemType Directory -Path $runDir -Force | Out-Null

    $status = "Passed"
    if (@($Checks | Where-Object { $_.Status -eq "Failed" -or $_.Status -eq "Blocked" }).Count -gt 0) {
        $status = "Failed"
    }

    $startedAt = (Get-Date).ToString("o")
    $report = [pscustomobject][ordered]@{
        runId       = $runId
        startedAt   = $startedAt
        mode        = $Mode
        status      = $status
        repoRoot    = $RepoRoot
        environment = @($Environment)
        checks      = @($Checks)
    }

    $jsonPath = Join-Path $runDir "report.json"
    $markdownPath = Join-Path $runDir "report.md"

    $report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8

    $lines = New-Object System.Collections.Generic.List[string]
    [void]$lines.Add("# Agent Harness Report")
    [void]$lines.Add("")
    [void]$lines.Add("- Run ID: ``$runId``")
    [void]$lines.Add("- Mode: ``$Mode``")
    [void]$lines.Add("- Status: ``$status``")
    [void]$lines.Add("- Repo: ``$RepoRoot``")
    [void]$lines.Add("")
    [void]$lines.Add("## Environment")
    [void]$lines.Add("")
    [void]$lines.Add("| Name | Status | Value |")
    [void]$lines.Add("| --- | --- | --- |")
    foreach ($item in $Environment) {
        [void]$lines.Add("| $($item.Name) | $($item.Status) | $($item.Value -replace '\|', '/') |")
    }
    [void]$lines.Add("")
    [void]$lines.Add("## Checks")
    [void]$lines.Add("")
    [void]$lines.Add("| ID | Status | Severity | Message |")
    [void]$lines.Add("| --- | --- | --- | --- |")
    foreach ($check in $Checks) {
        [void]$lines.Add("| $($check.Id) | $($check.Status) | $($check.Severity) | $($check.Message -replace '\|', '/') |")
    }
    [void]$lines.Add("")
    [void]$lines.Add("## Evidence")
    foreach ($check in $Checks) {
        [void]$lines.Add("")
        [void]$lines.Add("### $($check.Id)")
        if (@($check.Evidence).Count -eq 0) {
            [void]$lines.Add("- No evidence recorded.")
        }
        else {
            foreach ($evidence in $check.Evidence) {
                [void]$lines.Add("- $evidence")
            }
        }
    }

    $lines | Set-Content -LiteralPath $markdownPath -Encoding UTF8

    [pscustomobject][ordered]@{
        RunId        = $runId
        Status       = $status
        MarkdownPath = $markdownPath
        JsonPath     = $jsonPath
    }
}

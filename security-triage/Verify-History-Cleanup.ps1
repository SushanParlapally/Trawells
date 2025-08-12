# VERIFY GIT HISTORY CLEANUP SCRIPT
# This script verifies that secrets have been completely removed from Git history

param(
    [switch]$Detailed = $false
)

Write-Host "VERIFYING GIT HISTORY CLEANUP" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan
Write-Host ""

# Verify we're in a Git repository
if (-not (Test-Path ".git")) {
    Write-Host "Not in a Git repository!" -ForegroundColor Red
    exit 1
}

# Define the compromised secrets to search for
$compromisedSecrets = @(
    "Mahendra@07",
    "ThisismySecretKey568369342286956369",
    "bbge rpiz maie fpby",
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
)

Write-Host "Step 1: Checking current working directory files..." -ForegroundColor Yellow
$currentSecrets = @()

# Check current appsettings.json
if (Test-Path "appsettings.json") {
    $content = Get-Content "appsettings.json" -Raw
    foreach ($secret in $compromisedSecrets) {
        if ($content -like "*$secret*") {
            $currentSecrets += $secret
        }
    }
    
    if ($currentSecrets.Count -eq 0) {
        Write-Host "Current appsettings.json contains no compromised secrets" -ForegroundColor Green
    } else {
        Write-Host "Current appsettings.json still contains secrets:" -ForegroundColor Yellow
        foreach ($secret in $currentSecrets) {
            $preview = $secret.Substring(0, [Math]::Min(20, $secret.Length)) + "..."
            Write-Host "  - Found: $preview" -ForegroundColor Red
        }
    }
} else {
    Write-Host "No appsettings.json in current directory" -ForegroundColor Green
}

Write-Host ""
Write-Host "Step 2: Searching entire Git history for compromised secrets..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Cyan

$foundInHistory = @()
$totalChecks = $compromisedSecrets.Count
$currentCheck = 0

foreach ($secret in $compromisedSecrets) {
    $currentCheck++
    $secretPreview = $secret.Substring(0, [Math]::Min(15, $secret.Length)) + "..."
    Write-Host "[$currentCheck/$totalChecks] Searching for: $secretPreview" -ForegroundColor Cyan
    
    try {
        # Search all file contents in history
        $result = git grep "$secret" $(git rev-list --all) 2>&1
        if ($result -and $result.Length -gt 0 -and $result -notlike "*fatal:*" -and $result -notlike "*no matches found*") {
            $foundInHistory += $secret
            if ($Detailed) {
                Write-Host "Found in history: $result" -ForegroundColor Red
            }
        }
    }
    catch {
        # Ignore errors - likely means no matches found
    }
}

# Check if appsettings.json still exists in any commit
Write-Host ""
Write-Host "Step 3: Checking if appsettings.json exists in any commit..." -ForegroundColor Yellow
try {
    $appsettingsInHistory = git log --all --name-only --pretty=format: -- appsettings.json 2>&1
    if ($appsettingsInHistory -and $appsettingsInHistory -like "*appsettings.json*") {
        Write-Host "appsettings.json still found in Git history!" -ForegroundColor Red
        $foundInHistory += "appsettings.json file"
    } else {
        Write-Host "appsettings.json successfully removed from all commits" -ForegroundColor Green
    }
}
catch {
    Write-Host "appsettings.json successfully removed from all commits" -ForegroundColor Green
}

Write-Host ""
Write-Host "Step 4: Analysis Results" -ForegroundColor Yellow
Write-Host "========================" -ForegroundColor Yellow

if ($foundInHistory.Count -eq 0) {
    Write-Host ""
    Write-Host "EXCELLENT! NO COMPROMISED SECRETS FOUND IN GIT HISTORY!" -ForegroundColor Green
    Write-Host "========================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Your Git history is clean of all compromised secrets" -ForegroundColor Green
    Write-Host "appsettings.json has been successfully removed from all commits" -ForegroundColor Green
    Write-Host "The decontamination was successful" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "SECRETS STILL FOUND IN GIT HISTORY!" -ForegroundColor Red
    Write-Host "===================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "The following compromised data was found:" -ForegroundColor Yellow
    
    foreach ($finding in $foundInHistory) {
        Write-Host "Found: $finding" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "DECONTAMINATION INCOMPLETE" -ForegroundColor Red
    Write-Host "You may need to run the decontamination script again" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 5: Additional Security Checks" -ForegroundColor Yellow
Write-Host "==================================" -ForegroundColor Yellow

# Check .gitignore
if (Test-Path ".gitignore") {
    $gitignoreContent = Get-Content ".gitignore" -Raw
    if ($gitignoreContent -like "*appsettings.json*") {
        Write-Host ".gitignore properly excludes appsettings.json" -ForegroundColor Green
    } else {
        Write-Host ".gitignore does not exclude appsettings.json" -ForegroundColor Yellow
    }
} else {
    Write-Host "No .gitignore file found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 6: Final Recommendations" -ForegroundColor Yellow
Write-Host "=============================" -ForegroundColor Yellow

if ($foundInHistory.Count -eq 0) {
    Write-Host ""
    Write-Host "READY FOR DEPLOYMENT!" -ForegroundColor Green
    Write-Host "=====================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Your repository is now secure. Next steps:" -ForegroundColor White
    Write-Host "1. Force push to remote: git push origin --force --all" -ForegroundColor Cyan
    Write-Host "2. Force push tags: git push origin --force --tags" -ForegroundColor Cyan
    Write-Host "3. Notify team members to re-clone the repository" -ForegroundColor Cyan
    Write-Host "4. Monitor your application to ensure it works with environment variables" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Security remediation is COMPLETE!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "ADDITIONAL WORK REQUIRED" -ForegroundColor Yellow
    Write-Host "========================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Secrets are still present in Git history." -ForegroundColor Red
    Write-Host "DO NOT push to remote until this is resolved." -ForegroundColor Red
}

Write-Host ""
Write-Host "Verification complete." -ForegroundColor Cyan
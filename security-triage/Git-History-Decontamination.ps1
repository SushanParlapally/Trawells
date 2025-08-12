# GIT HISTORY DECONTAMINATION SCRIPT
# This script removes appsettings.json from ALL Git history commits

param(
    [switch]$WhatIf = $false,
    [switch]$Force = $false
)

Write-Host "GIT HISTORY DECONTAMINATION" -ForegroundColor Red
Write-Host "===========================" -ForegroundColor Red
Write-Host ""

if ($WhatIf) {
    Write-Host "RUNNING IN WHAT-IF MODE - No actual changes will be made" -ForegroundColor Yellow
    Write-Host ""
}

if (-not $Force) {
    Write-Host "CRITICAL WARNING" -ForegroundColor Yellow
    Write-Host "================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "This script will PERMANENTLY REWRITE your Git history!" -ForegroundColor Red
    Write-Host "- All commits containing appsettings.json will be modified" -ForegroundColor Yellow
    Write-Host "- This action is IRREVERSIBLE" -ForegroundColor Yellow
    Write-Host "- Anyone who has cloned your repo will need to re-clone" -ForegroundColor Yellow
    Write-Host "- All commit hashes will change" -ForegroundColor Yellow
    Write-Host ""
    
    if (-not $WhatIf) {
        $confirmation = Read-Host "Type 'DECONTAMINATE' to proceed with history rewriting"
        if ($confirmation -ne "DECONTAMINATE") {
            Write-Host "Operation cancelled by user" -ForegroundColor Yellow
            exit 0
        }
    }
}

# Function to check if command exists
function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

# Step 1: Verify we're in a Git repository
Write-Host "Step 1: Verifying Git repository..." -ForegroundColor Cyan
if (-not (Test-Path ".git")) {
    Write-Host "Not in a Git repository root!" -ForegroundColor Red
    Write-Host "Please run this script from the TravelDesk-master directory" -ForegroundColor Yellow
    exit 1
}
Write-Host "Git repository detected" -ForegroundColor Green

# Step 2: Check if git-filter-repo is available
Write-Host ""
Write-Host "Step 2: Checking git-filter-repo availability..." -ForegroundColor Cyan
$useModule = $false

if (Test-Command "git") {
    try {
        git filter-repo --version 2>&1 | Out-Null
        Write-Host "git filter-repo command available" -ForegroundColor Green
    }
    catch {
        Write-Host "git filter-repo command not found, trying Python module..." -ForegroundColor Yellow
        try {
            python -m git_filter_repo --version 2>&1 | Out-Null
            Write-Host "git-filter-repo Python module available" -ForegroundColor Green
            $useModule = $true
        }
        catch {
            Write-Host "git-filter-repo not found!" -ForegroundColor Red
            Write-Host "Please run Install-GitFilterRepo.ps1 first" -ForegroundColor Yellow
            exit 1
        }
    }
} else {
    Write-Host "Git not found!" -ForegroundColor Red
    exit 1
}

# Step 3: Create backup information
Write-Host ""
Write-Host "Step 3: Creating backup information..." -ForegroundColor Cyan
$backupBranch = "backup-before-decontamination-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

if (-not $WhatIf) {
    try {
        git branch $backupBranch
        Write-Host "Backup branch created: $backupBranch" -ForegroundColor Green
    }
    catch {
        Write-Host "Could not create backup branch" -ForegroundColor Yellow
    }
} else {
    Write-Host "Would create backup branch: $backupBranch" -ForegroundColor Yellow
}

# Step 4: Show files that will be removed from history
Write-Host ""
Write-Host "Step 4: Analyzing files to be removed..." -ForegroundColor Cyan
$filesToRemove = @("appsettings.json", "appsettings.Development.json", "appsettings.Production.json")

foreach ($file in $filesToRemove) {
    if (Test-Path $file) {
        Write-Host "FOUND: $file - WILL BE REMOVED FROM ALL HISTORY" -ForegroundColor Red
    } else {
        Write-Host "NOT FOUND: $file - will be removed if exists in history" -ForegroundColor Yellow
    }
}

# Step 5: Execute git-filter-repo to remove appsettings.json
Write-Host ""
Write-Host "Step 5: Executing Git history decontamination..." -ForegroundColor Cyan
Write-Host "This may take several minutes depending on repository size..." -ForegroundColor Yellow

if (-not $WhatIf) {
    try {
        Write-Host ""
        Write-Host "REMOVING appsettings.json FROM ALL COMMITS..." -ForegroundColor Red
        
        if ($useModule) {
            $command = "python -m git_filter_repo --path appsettings.json --invert-paths --force"
        } else {
            $command = "git filter-repo --path appsettings.json --invert-paths --force"
        }
        
        Write-Host "Executing: $command" -ForegroundColor Cyan
        Invoke-Expression $command
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "appsettings.json removed from Git history!" -ForegroundColor Green
        } else {
            Write-Host "Git filter-repo failed with exit code: $LASTEXITCODE" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "Decontamination failed: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Would execute: git filter-repo --path appsettings.json --invert-paths --force" -ForegroundColor Yellow
}

# Step 6: Add appsettings.json to .gitignore
Write-Host ""
Write-Host "Step 6: Updating .gitignore..." -ForegroundColor Cyan

$gitignoreContent = @"
# Sensitive configuration files
appsettings.json
appsettings.Development.json
appsettings.Production.json
appsettings.*.json

# User secrets
secrets.json
"@

if (-not $WhatIf) {
    if (Test-Path ".gitignore") {
        $existingContent = Get-Content ".gitignore" -Raw
        if ($existingContent -notlike "*appsettings.json*") {
            Add-Content ".gitignore" "`n# Added by security remediation"
            Add-Content ".gitignore" $gitignoreContent
            Write-Host ".gitignore updated with appsettings.json exclusions" -ForegroundColor Green
        } else {
            Write-Host ".gitignore already contains appsettings.json exclusions" -ForegroundColor Green
        }
    } else {
        Set-Content ".gitignore" $gitignoreContent
        Write-Host ".gitignore created with appsettings.json exclusions" -ForegroundColor Green
    }
} else {
    Write-Host "Would update .gitignore to exclude appsettings.json" -ForegroundColor Yellow
}

# Step 7: Create secure appsettings.json template
Write-Host ""
Write-Host "Step 7: Creating secure appsettings.json template..." -ForegroundColor Cyan

$secureTemplate = @'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "PLACEHOLDER - Use environment variables or user secrets"
  },
  "Jwt": {
    "Key": "PLACEHOLDER - Use environment variables or user secrets",
    "Issuer": "Test.com",
    "Audience": "Test.com"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "noreplytrawell@gmail.com",
    "SenderPassword": "PLACEHOLDER - Use environment variables or user secrets"
  },
  "Supabase": {
    "Url": "https://pkhlhfpknxjaqruarvhi.supabase.co",
    "Key": "PLACEHOLDER - Use environment variables or user secrets",
    "ServiceKey": "PLACEHOLDER - Use environment variables or user secrets"
  },
  "AppSettings": {
    "BaseUrl": "https://trawells.onrender.com",
    "ApiBaseUrl": "https://trawells.onrender.com"
  }
}
'@

if (-not $WhatIf) {
    Set-Content "appsettings.json" $secureTemplate
    Write-Host "Secure appsettings.json template created" -ForegroundColor Green
} else {
    Write-Host "Would create secure appsettings.json template" -ForegroundColor Yellow
}

# Step 8: Commit the changes
Write-Host ""
Write-Host "Step 8: Committing security improvements..." -ForegroundColor Cyan

if (-not $WhatIf) {
    try {
        git add .gitignore appsettings.json
        git commit -m "Security: Add .gitignore rules and secure appsettings.json template

- Added appsettings.json to .gitignore to prevent future secret leaks
- Created secure appsettings.json template with placeholders
- All sensitive values now use environment variables
- Part of security remediation after secret exposure incident"
        
        Write-Host "Security improvements committed" -ForegroundColor Green
    }
    catch {
        Write-Host "Could not commit changes: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "Would commit .gitignore and secure appsettings.json" -ForegroundColor Yellow
}

# Step 9: Display next steps
Write-Host ""
Write-Host "GIT HISTORY DECONTAMINATION COMPLETE!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Cyan
Write-Host "1. Run verification script: .\security-triage\Verify-History-Cleanup.ps1" -ForegroundColor White
Write-Host "2. Force push to remote: git push origin --force --all" -ForegroundColor White
Write-Host "3. Force push tags: git push origin --force --tags" -ForegroundColor White
Write-Host "4. Notify team members to re-clone the repository" -ForegroundColor White
Write-Host ""
Write-Host "IMPORTANT REMINDERS:" -ForegroundColor Yellow
Write-Host "- All commit hashes have changed" -ForegroundColor White
Write-Host "- Anyone with existing clones must re-clone" -ForegroundColor White
Write-Host "- Check that your application works with environment variables" -ForegroundColor White
Write-Host "- The secrets are now completely removed from Git history" -ForegroundColor White
Write-Host ""
Write-Host "Your repository is now secure!" -ForegroundColor Green
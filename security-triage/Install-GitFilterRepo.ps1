# GIT-FILTER-REPO INSTALLATION SCRIPT FOR WINDOWS
# This script installs git-filter-repo tool for Git history decontamination

param(
    [switch]$WhatIf = $false
)

Write-Host "GIT-FILTER-REPO INSTALLATION" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan
Write-Host ""

if ($WhatIf) {
    Write-Host "RUNNING IN WHAT-IF MODE - No actual installation will be performed" -ForegroundColor Yellow
    Write-Host ""
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

# Step 1: Check if Python is installed
Write-Host "Step 1: Checking Python installation..." -ForegroundColor Yellow
if (Test-Command "python") {
    $pythonVersion = python --version 2>&1
    Write-Host "Python found: $pythonVersion" -ForegroundColor Green
} elseif (Test-Command "python3") {
    $pythonVersion = python3 --version 2>&1
    Write-Host "Python3 found: $pythonVersion" -ForegroundColor Green
    Set-Alias -Name python -Value python3 -Scope Global
} else {
    Write-Host "Python not found!" -ForegroundColor Red
    Write-Host "Please install Python from: https://www.python.org/downloads/" -ForegroundColor Red
    Write-Host "Make sure to check 'Add Python to PATH' during installation" -ForegroundColor Yellow
    exit 1
}

# Step 2: Check if pip is available
Write-Host ""
Write-Host "Step 2: Checking pip installation..." -ForegroundColor Yellow
if (Test-Command "pip") {
    $pipVersion = pip --version 2>&1
    Write-Host "pip found: $pipVersion" -ForegroundColor Green
} elseif (Test-Command "pip3") {
    $pipVersion = pip3 --version 2>&1
    Write-Host "pip3 found: $pipVersion" -ForegroundColor Green
    Set-Alias -Name pip -Value pip3 -Scope Global
} else {
    Write-Host "pip not found!" -ForegroundColor Red
    Write-Host "Please install pip or reinstall Python with pip included" -ForegroundColor Red
    exit 1
}

# Step 3: Install git-filter-repo
Write-Host ""
Write-Host "Step 3: Installing git-filter-repo..." -ForegroundColor Yellow

if (-not $WhatIf) {
    try {
        Write-Host "Running: pip install git-filter-repo" -ForegroundColor Cyan
        pip install git-filter-repo
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "git-filter-repo installed successfully!" -ForegroundColor Green
        } else {
            Write-Host "Installation failed with exit code: $LASTEXITCODE" -ForegroundColor Red
            exit 1
        }
    }
    catch {
        Write-Host "Installation failed: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Would run: pip install git-filter-repo" -ForegroundColor Yellow
}

# Step 4: Verify installation
Write-Host ""
Write-Host "Step 4: Verifying installation..." -ForegroundColor Yellow

if (-not $WhatIf) {
    try {
        $filterRepoVersion = git filter-repo --version 2>&1
        Write-Host "git-filter-repo is working: $filterRepoVersion" -ForegroundColor Green
    }
    catch {
        Write-Host "git-filter-repo command not found in PATH" -ForegroundColor Yellow
        Write-Host "Trying alternative verification..." -ForegroundColor Yellow
        
        try {
            python -m git_filter_repo --version 2>&1 | Out-Null
            Write-Host "git-filter-repo is available via python module" -ForegroundColor Green
            Write-Host "Use 'python -m git_filter_repo' instead of 'git filter-repo'" -ForegroundColor Cyan
        }
        catch {
            Write-Host "git-filter-repo verification failed" -ForegroundColor Red
            Write-Host "Try running the installation again or install manually" -ForegroundColor Yellow
            exit 1
        }
    }
} else {
    Write-Host "Would verify: git filter-repo --version" -ForegroundColor Yellow
}

# Step 5: Installation complete
Write-Host ""
Write-Host "INSTALLATION COMPLETE!" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green
Write-Host ""
Write-Host "git-filter-repo is now ready for Git history decontamination" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Run the Git decontamination script" -ForegroundColor White
Write-Host "2. Remove appsettings.json from Git history" -ForegroundColor White
Write-Host "3. Verify secrets are completely removed" -ForegroundColor White
Write-Host ""
Write-Host "WARNING: Git history rewriting is irreversible!" -ForegroundColor Yellow
Write-Host "Make sure you have backups before proceeding." -ForegroundColor Yellow
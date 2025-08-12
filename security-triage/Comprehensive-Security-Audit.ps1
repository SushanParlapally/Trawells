#!/usr/bin/env pwsh

Write-Host "Comprehensive Security Audit" -ForegroundColor Green
Write-Host "============================" -ForegroundColor Green

$foundIssues = $false
$issueCount = 0

# Function to report issues
function Report-Issue {
    param($message, $severity = "ERROR")
    $script:foundIssues = $true
    $script:issueCount++
    $color = if ($severity -eq "ERROR") { "Red" } elseif ($severity -eq "WARNING") { "Yellow" } else { "Cyan" }
    Write-Host "   [$severity] $message" -ForegroundColor $color
}

function Report-Success {
    param($message)
    Write-Host "   [OK] $message" -ForegroundColor Green
}

# Backend Security Checks
Write-Host "`nBackend Security Audit" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

# Check 1: JWT Configuration
Write-Host "`n1. JWT Configuration Security..." -ForegroundColor Yellow
$loginControllerPath = Join-Path (Join-Path (Join-Path $PSScriptRoot "..") "Controllers") "LoginController.cs"
if (Test-Path $loginControllerPath) {
    $loginContent = Get-Content $loginControllerPath -Raw
    
    # Check if JWT key is read from environment
    if ($loginContent -match "Environment\.GetEnvironmentVariable\(`"JWT_KEY`"\)") {
        Report-Success "JWT key read from environment variable"
    } else {
        Report-Issue "JWT key not read from environment variable"
    }
    
    # Check if JWT issuer/audience are hardcoded (not critical but should be consistent)
    if ($loginContent -match "_configuration\[`"Jwt:Issuer`"\]") {
        Report-Success "JWT issuer/audience from configuration (acceptable for non-sensitive values)"
    }
} else {
    Report-Issue "LoginController.cs not found"
}

# Check 2: Database Connection Security
Write-Host "`n2. Database Connection Security..." -ForegroundColor Yellow
$programPath = Join-Path (Join-Path $PSScriptRoot "..") "Program.cs"
if (Test-Path $programPath) {
    $programContent = Get-Content $programPath -Raw
    
    # Check if DATABASE_URL environment variable is used
    if ($programContent -match "Environment\.GetEnvironmentVariable\(`"DATABASE_URL`"\)") {
        Report-Success "Database connection uses environment variable"
    } else {
        Report-Issue "Database connection not using environment variable"
    }
    
    # Check if connection string is logged
    if ($programContent -match "Console\.WriteLine.*connection.*:") {
        Report-Issue "Connection string being logged (security risk)"
    } else {
        Report-Success "No connection string logging found"
    }
} else {
    Report-Issue "Program.cs not found"
}

# Check 3: Email Configuration Security
Write-Host "`n3. Email Configuration Security..." -ForegroundColor Yellow
if (Test-Path $programPath) {
    $programContent = Get-Content $programPath -Raw
    
    # Check if email password is read from environment
    if ($programContent -match "Environment\.GetEnvironmentVariable\(`"EMAIL_PASSWORD`"\)") {
        Report-Success "Email password read from environment variable"
    } else {
        Report-Issue "Email password not read from environment variable"
    }
}

# Check 4: Supabase Configuration Security
Write-Host "`n4. Supabase Configuration Security..." -ForegroundColor Yellow
$supabaseServicePath = Join-Path (Join-Path $PSScriptRoot "..") "Service/SupabaseStorageService.cs"
if (Test-Path $supabaseServicePath) {
    $supabaseContent = Get-Content $supabaseServicePath -Raw
    
    # Check if Supabase key is read from environment
    if ($supabaseContent -match "Environment\.GetEnvironmentVariable\(`"SUPABASE_ANON_KEY`"\)") {
        Report-Success "Supabase key read from environment variable"
    } else {
        Report-Issue "Supabase key not read from environment variable"
    }
} else {
    Report-Issue "SupabaseStorageService.cs not found"
}

# Check 5: Configuration Files Security
Write-Host "`n5. Configuration Files Security..." -ForegroundColor Yellow
$appsettingsPath = Join-Path (Join-Path $PSScriptRoot "..") "appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettingsContent = Get-Content $appsettingsPath -Raw
    
    # Check for placeholder values instead of real secrets
    if ($appsettingsContent -match "PLACEHOLDER") {
        Report-Success "Configuration files use placeholders for sensitive data"
    } else {
        Report-Issue "Configuration files may contain real secrets"
    }
    
    # Check for specific sensitive patterns
    $sensitivePatterns = @(
        "password.*:.*[^PLACEHOLDER]",
        "key.*:.*[^PLACEHOLDER]",
        "secret.*:.*[^PLACEHOLDER]"
    )
    
    foreach ($pattern in $sensitivePatterns) {
        if ($appsettingsContent -match $pattern -and $appsettingsContent -notmatch "PLACEHOLDER") {
            Report-Issue "Potential sensitive data in appsettings.json"
            break
        }
    }
}

# Frontend Security Checks
Write-Host "`nFrontend Security Audit" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

# Check 6: Environment Variable Usage
Write-Host "`n6. Frontend Environment Variable Usage..." -ForegroundColor Yellow
$frontendConfigPath = Join-Path $PSScriptRoot "../../travel-desk-frontend/src/services/api/config.ts"
if (Test-Path $frontendConfigPath) {
    $frontendConfigContent = Get-Content $frontendConfigPath -Raw
    
    # Check if API base URL uses environment variable
    if ($frontendConfigContent -match "import\.meta\.env\[.*VITE_API_BASE_URL.*\]") {
        Report-Success "Frontend uses environment variable for API base URL"
    } else {
        Report-Issue "Frontend not using environment variable for API base URL"
    }
    
    # Check for hardcoded localhost URLs
    if ($frontendConfigContent -match "localhost|127\.0\.0\.1") {
        Report-Issue "Hardcoded localhost URLs found in frontend"
    } else {
        Report-Success "No hardcoded localhost URLs in frontend config"
    }
} else {
    Report-Issue "Frontend config.ts not found at expected location"
}

# Check 7: Frontend Secret Storage
Write-Host "`n7. Frontend Secret Storage..." -ForegroundColor Yellow
$authServicePath = Join-Path $PSScriptRoot "../../travel-desk-frontend/src/services/auth/authService.ts"
if (Test-Path $authServicePath) {
    $authServiceContent = Get-Content $authServicePath -Raw
    
    # Check that passwords are not stored
    if ($authServiceContent -match "password.*:.*''.*Never store password") {
        Report-Success "Frontend does not store passwords"
    } else {
        Report-Issue "Frontend may be storing passwords"
    }
    
    # Check for secure token storage
    if ($authServiceContent -match "TOKEN_KEY.*=.*'travel_desk_token'") {
        Report-Success "Frontend uses consistent token storage key"
    }
} else {
    Report-Issue "Frontend authService.ts not found at expected location"
}

# Check 8: Git Security
Write-Host "`n8. Git Repository Security..." -ForegroundColor Yellow

# Check if .env files are in .gitignore
$frontendGitignorePath = Join-Path $PSScriptRoot "../../travel-desk-frontend/.gitignore"
if (Test-Path $frontendGitignorePath) {
    $gitignoreContent = Get-Content $frontendGitignorePath -Raw
    if ($gitignoreContent -match "\.env") {
        Report-Success "Frontend .gitignore includes .env files"
    } else {
        Report-Issue "Frontend .gitignore missing .env files"
    }
}

$backendGitignorePath = Join-Path (Join-Path $PSScriptRoot "..") ".gitignore"
if (Test-Path $backendGitignorePath) {
    $gitignoreContent = Get-Content $backendGitignorePath -Raw
    if ($gitignoreContent -match "appsettings.*\.json") {
        Report-Success "Backend .gitignore includes appsettings files"
    } else {
        Report-Issue "Backend .gitignore missing appsettings files"
    }
}

# Summary
Write-Host "`nSecurity Audit Summary" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

if (-not $foundIssues) {
    Write-Host "All security checks passed!" -ForegroundColor Green
    Write-Host "Your application follows security best practices." -ForegroundColor Green
} else {
    Write-Host "Found $issueCount security issues that need attention." -ForegroundColor Red
    Write-Host "Please review the issues above and fix them before deployment." -ForegroundColor Red
}

Write-Host "`nEnvironment Variables Required:" -ForegroundColor Cyan
Write-Host "- JWT_KEY: JWT signing key" -ForegroundColor White
Write-Host "- DATABASE_URL: PostgreSQL connection string" -ForegroundColor White
Write-Host "- EMAIL_PASSWORD: Gmail app password" -ForegroundColor White
Write-Host "- SUPABASE_ANON_KEY: Supabase anonymous key" -ForegroundColor White
Write-Host "- VITE_API_BASE_URL: Frontend API base URL (frontend only)" -ForegroundColor White

Write-Host "`nSecurity audit complete!" -ForegroundColor Green
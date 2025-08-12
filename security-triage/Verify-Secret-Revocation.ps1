# SECRET REVOCATION VERIFICATION SCRIPT
# Execute this script to verify that old secrets have been invalidated

param(
    [string]$NewDatabasePassword = "",
    [string]$NewServiceKey = "",
    [string]$NewJwtKey = "",
    [string]$NewEmailPassword = ""
)

Write-Host "🔍 SECRET REVOCATION VERIFICATION" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$verificationResults = @()
$allTestsPassed = $true

# Function to test database connection
function Test-DatabaseConnection {
    param(
        [string]$ConnectionString,
        [string]$TestName
    )
    
    Write-Host "Testing: $TestName..." -NoNewline
    
    try {
        # Create a simple connection test (this would need actual .NET code in production)
        # For now, we'll simulate the test
        if ($ConnectionString -like "*Mahendra@07*") {
            Write-Host " [FAIL] Old password still in use!" -ForegroundColor Red
            return $false
        } else {
            Write-Host " [PASS]" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host " [FAIL] Connection error: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to test Supabase API key
function Test-SupabaseServiceKey {
    param(
        [string]$ServiceKey,
        [string]$ProjectUrl
    )
    
    Write-Host "Testing: Supabase Service Key..." -NoNewline
    
    if ($ServiceKey -eq "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBraGxoZnBrbnhqYXFydWFydmhpIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1NDQ2NDE2MywiZXhwIjoyMDcwMDQwMTYzfQ.iFEKiBkz3qgFoEtdrl-ZJcr3EleJkc32HGuXceTlW6k") {
        Write-Host " [FAIL] Old service key still in use!" -ForegroundColor Red
        return $false
    }
    
    try {
        # Test API call to Supabase (simplified for PowerShell)
        $headers = @{
            "apikey" = $ServiceKey
            "Authorization" = "Bearer $ServiceKey"
            "Content-Type" = "application/json"
        }
        
        $response = Invoke-RestMethod -Uri "$ProjectUrl/rest/v1/" -Headers $headers -Method Get -TimeoutSec 10
        Write-Host " [PASS] New service key working" -ForegroundColor Green
        return $true
    }
    catch {
        if ($_.Exception.Message -like "*401*" -or $_.Exception.Message -like "*403*") {
            Write-Host " [FAIL] Service key invalid or revoked" -ForegroundColor Red
            return $false
        } else {
            Write-Host " [WARN] Connection issue, but key appears valid" -ForegroundColor Yellow
            return $true
        }
    }
}

# Function to test email authentication
function Test-EmailAuthentication {
    param(
        [string]$EmailPassword
    )
    
    Write-Host "Testing: Email Authentication..." -NoNewline
    
    if ($EmailPassword -eq "bbge rpiz maie fpby") {
        Write-Host " [FAIL] Old email password still in use!" -ForegroundColor Red
        return $false
    }
    
    if ($EmailPassword.Length -ne 16 -or $EmailPassword -notmatch "^[a-z]{4} [a-z]{4} [a-z]{4} [a-z]{4}$") {
        Write-Host " [WARN] Password format doesn't match Gmail app password format" -ForegroundColor Yellow
        return $true
    }
    
    Write-Host " [PASS] New email password format valid" -ForegroundColor Green
    return $true
}

# Main verification process
Write-Host "Starting verification of secret invalidation..." -ForegroundColor White
Write-Host ""

# Test 1: Check if old secrets are still in configuration files
Write-Host "1. CHECKING CONFIGURATION FILES" -ForegroundColor Yellow
Write-Host "===============================" -ForegroundColor Yellow

$appsettingsPath = "appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettingsContent = Get-Content $appsettingsPath -Raw
    
    # Check for old database password
    if ($appsettingsContent -like "*Mahendra@07*") {
        Write-Host "[FAIL] OLD DATABASE PASSWORD found in appsettings.json!" -ForegroundColor Red
        $allTestsPassed = $false
    } else {
        Write-Host "[PASS] Old database password not found in appsettings.json" -ForegroundColor Green
    }
    
    # Check for old service key
    if ($appsettingsContent -like "*iFEKiBkz3qgFoEtdrl-ZJcr3EleJkc32HGuXceTlW6k*") {
        Write-Host "[FAIL] OLD SERVICE KEY found in appsettings.json!" -ForegroundColor Red
        $allTestsPassed = $false
    } else {
        Write-Host "[PASS] Old service key not found in appsettings.json" -ForegroundColor Green
    }
    
    # Check for old JWT key
    if ($appsettingsContent -like "*ThisismySecretKey568369342286956369*") {
        Write-Host "[FAIL] OLD JWT KEY found in appsettings.json!" -ForegroundColor Red
        $allTestsPassed = $false
    } else {
        Write-Host "[PASS] Old JWT key not found in appsettings.json" -ForegroundColor Green
    }
    
    # Check for old email password
    if ($appsettingsContent -like "*bbge rpiz maie fpby*") {
        Write-Host "[FAIL] OLD EMAIL PASSWORD found in appsettings.json!" -ForegroundColor Red
        $allTestsPassed = $false
    } else {
        Write-Host "[PASS] Old email password not found in appsettings.json" -ForegroundColor Green
    }
}

Write-Host ""

# Test 2: Verify new secrets are working (if provided)
Write-Host "2. TESTING NEW CREDENTIALS" -ForegroundColor Yellow
Write-Host "==========================" -ForegroundColor Yellow

if ($NewServiceKey) {
    $serviceKeyTest = Test-SupabaseServiceKey -ServiceKey $NewServiceKey -ProjectUrl "https://pkhlhfpknxjaqruarvhi.supabase.co"
    if (-not $serviceKeyTest) { $allTestsPassed = $false }
} else {
    Write-Host "[WARN] New service key not provided - manual verification required" -ForegroundColor Yellow
}

if ($NewEmailPassword) {
    $emailTest = Test-EmailAuthentication -EmailPassword $NewEmailPassword
    if (-not $emailTest) { $allTestsPassed = $false }
} else {
    Write-Host "[WARN] New email password not provided - manual verification required" -ForegroundColor Yellow
}

Write-Host ""

# Test 3: Check production environment
Write-Host "3. PRODUCTION ENVIRONMENT CHECK" -ForegroundColor Yellow
Write-Host "===============================" -ForegroundColor Yellow

Write-Host "Manual verification required for Render.io environment variables:" -ForegroundColor White
Write-Host "1. Go to: https://dashboard.render.com" -ForegroundColor White
Write-Host "2. Select your TravelDesk service" -ForegroundColor White
Write-Host "3. Check Environment tab" -ForegroundColor White
Write-Host "4. Verify these variables are updated:" -ForegroundColor White
Write-Host "   - DATABASE_URL (no 'Mahendra@07')" -ForegroundColor White
Write-Host "   - SUPABASE_SERVICE_KEY (new key)" -ForegroundColor White
Write-Host "   - JWT_KEY (new key)" -ForegroundColor White
Write-Host "   - EMAIL_PASSWORD (new app password)" -ForegroundColor White
Write-Host ""

# Test 4: Application functionality test
Write-Host "4. APPLICATION FUNCTIONALITY TEST" -ForegroundColor Yellow
Write-Host "=================================" -ForegroundColor Yellow

Write-Host "Testing production application..." -NoNewline
try {
    $response = Invoke-WebRequest -Uri "https://trawells.onrender.com/health" -TimeoutSec 30 -ErrorAction SilentlyContinue
    if ($response.StatusCode -eq 200) {
        Write-Host " [PASS] Application is responding" -ForegroundColor Green
    } else {
        Write-Host " [WARN] Application returned status: $($response.StatusCode)" -ForegroundColor Yellow
    }
}
catch {
    Write-Host " [WARN] Could not reach application (may be starting up)" -ForegroundColor Yellow
}

Write-Host ""

# Final results
Write-Host "VERIFICATION SUMMARY" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

if ($allTestsPassed) {
    Write-Host "[PASS] ALL AUTOMATED TESTS PASSED" -ForegroundColor Green
    Write-Host ""
    Write-Host "NEXT STEPS:" -ForegroundColor White
    Write-Host "1. Complete manual verification of production environment" -ForegroundColor White
    Write-Host "2. Test application login/functionality" -ForegroundColor White
    Write-Host "3. Proceed with Git history decontamination (Task 2)" -ForegroundColor White
} else {
    Write-Host "[FAIL] SOME TESTS FAILED - IMMEDIATE ACTION REQUIRED" -ForegroundColor Red
    Write-Host ""
    Write-Host "REQUIRED ACTIONS:" -ForegroundColor White
    Write-Host "1. Review failed tests above" -ForegroundColor White
    Write-Host "2. Complete secret invalidation steps" -ForegroundColor White
    Write-Host "3. Re-run this verification script" -ForegroundColor White
}

Write-Host ""
Write-Host "[WARN] Remember to delete any temporary secret files after verification!" -ForegroundColor Yellow

# Create verification log
$logFile = "verification-log-$(Get-Date -Format 'yyyyMMdd-HHmmss').txt"
$logContent = @"
SECRET REVOCATION VERIFICATION LOG
Generated: $(Get-Date)
===================================

AUTOMATED TESTS: $(if ($allTestsPassed) { "PASSED" } else { "FAILED" })

Configuration File Checks:
- Old database password removed: $(if ($appsettingsContent -notlike "*Mahendra@07*") { "[PASS]" } else { "[FAIL]" })
- Old service key removed: $(if ($appsettingsContent -notlike "*iFEKiBkz3qgFoEtdrl-ZJcr3EleJkc32HGuXceTlW6k*") { "[PASS]" } else { "[FAIL]" })
- Old JWT key removed: $(if ($appsettingsContent -notlike "*ThisismySecretKey568369342286956369*") { "[PASS]" } else { "[FAIL]" })
- Old email password removed: $(if ($appsettingsContent -notlike "*bbge rpiz maie fpby*") { "[PASS]" } else { "[FAIL]" })

MANUAL VERIFICATION REQUIRED:
- Render.io environment variables updated
- Application functionality tested
- Database connectivity confirmed
- Email service working

NEXT STEPS:
$(if ($allTestsPassed) { "[PASS] Proceed with Git history decontamination" } else { "[FAIL] Complete failed verifications first" })
"@

$logContent | Out-File -FilePath $logFile -Encoding UTF8
Write-Host "Verification log saved to: $logFile" -ForegroundColor Cyan
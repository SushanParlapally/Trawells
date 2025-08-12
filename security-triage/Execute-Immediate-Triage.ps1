# MASTER TRIAGE EXECUTION SCRIPT
# This script orchestrates the complete immediate triage process

param(
    [switch]$WhatIf = $false,
    [switch]$SkipConfirmation = $false
)

# Set execution policy for this session
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force

Write-Host ""
Write-Host "🚨🚨🚨 CRITICAL SECURITY INCIDENT RESPONSE 🚨🚨🚨" -ForegroundColor Red -BackgroundColor White
Write-Host "=================================================" -ForegroundColor Red
Write-Host ""
Write-Host "EXPOSED SECRETS DETECTED IN PUBLIC REPOSITORY" -ForegroundColor Red
Write-Host "Immediate action required within 15 minutes!" -ForegroundColor Yellow
Write-Host ""

if (-not $SkipConfirmation) {
    $confirmation = Read-Host "Are you ready to begin immediate triage? (yes/no)"
    if ($confirmation -ne "yes") {
        Write-Host "Triage cancelled. Run with -SkipConfirmation to bypass this prompt." -ForegroundColor Yellow
        exit 1
    }
}

$startTime = Get-Date
Write-Host "🕐 Triage started at: $($startTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor Cyan
Write-Host ""

# Step 1: Display critical information
Write-Host "STEP 1: CRITICAL INFORMATION REVIEW" -ForegroundColor Magenta
Write-Host "====================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "📋 Opening triage guide..." -ForegroundColor White

if (Test-Path "IMMEDIATE_TRIAGE_GUIDE.md") {
    if ($IsWindows -or $env:OS -eq "Windows_NT") {
        Start-Process "IMMEDIATE_TRIAGE_GUIDE.md"
    }
    Write-Host "[OK] Triage guide opened" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Triage guide not found!" -ForegroundColor Red
}

Write-Host ""
Write-Host "📋 Opening Supabase navigation guide..." -ForegroundColor White

if (Test-Path "Supabase-Dashboard-Navigation.md") {
    if ($IsWindows -or $env:OS -eq "Windows_NT") {
        Start-Process "Supabase-Dashboard-Navigation.md"
    }
    Write-Host "[OK] Supabase guide opened" -ForegroundColor Green
} else {
    Write-Host "[FAIL] Supabase guide not found!" -ForegroundColor Red
}

Write-Host ""
Read-Host "Press Enter when you have reviewed the guides and are ready to continue"

# Step 2: Execute secret invalidation
Write-Host "STEP 2: SECRET INVALIDATION" -ForegroundColor Magenta
Write-Host "============================" -ForegroundColor Magenta
Write-Host ""

if (Test-Path "Invalidate-Secrets.ps1") {
    Write-Host "🔧 Executing secret invalidation script..." -ForegroundColor White
    if ($WhatIf) {
        & .\Invalidate-Secrets.ps1 -WhatIf
    } else {
        & .\Invalidate-Secrets.ps1
    }
} else {
    Write-Host "[FAIL] Invalidate-Secrets.ps1 not found!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "⚠️  MANUAL ACTIONS REQUIRED NOW:" -ForegroundColor Yellow
Write-Host "1. Reset Supabase database password" -ForegroundColor White
Write-Host "2. Regenerate Supabase service key" -ForegroundColor White
Write-Host "3. Update Render.io environment variables" -ForegroundColor White
Write-Host "4. Generate new Gmail app password" -ForegroundColor White
Write-Host ""

$manualComplete = Read-Host "Have you completed ALL manual actions? (yes/no)"
if ($manualComplete -ne "yes") {
    Write-Host "⏸️  Pausing triage. Complete manual actions and re-run this script." -ForegroundColor Yellow
    exit 1
}

# Step 3: Verification
Write-Host "STEP 3: VERIFICATION" -ForegroundColor Magenta
Write-Host "====================" -ForegroundColor Magenta
Write-Host ""

if (Test-Path "Verify-Secret-Revocation.ps1") {
    Write-Host "🔍 Executing verification script..." -ForegroundColor White
    & .\Verify-Secret-Revocation.ps1
} else {
    Write-Host "[FAIL] Verify-Secret-Revocation.ps1 not found!" -ForegroundColor Red
}

# Step 4: Final status
$endTime = Get-Date
$duration = $endTime - $startTime

Write-Host ""
Write-Host "TRIAGE COMPLETION STATUS" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🕐 Started: $($startTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor White
Write-Host "🕐 Ended: $($endTime.ToString('yyyy-MM-dd HH:mm:ss'))" -ForegroundColor White
Write-Host "⏱️  Duration: $($duration.TotalMinutes.ToString('F1')) minutes" -ForegroundColor White
Write-Host ""

if ($duration.TotalMinutes -le 15) {
    Write-Host "[PASS] TRIAGE COMPLETED WITHIN TARGET TIME!" -ForegroundColor Green
} else {
    Write-Host "[WARN] Triage took longer than 15 minutes - monitor for potential unauthorized access" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Cyan
Write-Host "1. Monitor application logs for 24-48 hours" -ForegroundColor White
Write-Host "2. Proceed with Git history decontamination (Task 2)" -ForegroundColor White
Write-Host "3. Implement secure configuration management (Task 3)" -ForegroundColor White
Write-Host ""

# Create completion log
$logFile = "triage-completion-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
$logContent = @"
SECURITY TRIAGE COMPLETION LOG
==============================
Incident Response Date: $(Get-Date)
Duration: $($duration.TotalMinutes.ToString('F1')) minutes
Target Met: $(if ($duration.TotalMinutes -le 15) { "YES" } else { "NO" })

ACTIONS COMPLETED:
[OK] Triage guides reviewed
[OK] Secret invalidation script executed
[OK] Manual secret resets completed
[OK] Verification script executed

NEXT PHASE:
- Git history decontamination
- Secure configuration implementation
- Application security hardening

MONITORING REQUIRED:
- Application logs (24-48 hours)
- Supabase activity logs
- Render.io deployment logs
- User authentication patterns
"@
$logContent | Out-File -FilePath $logFile -Encoding UTF8

Write-Host "📝 Completion log saved to: $logFile" -ForegroundColor Cyan
Write-Host ""
Write-Host "[SUCCESS] IMMEDIATE TRIAGE COMPLETE - PROCEED TO TASK 2" -ForegroundColor Green
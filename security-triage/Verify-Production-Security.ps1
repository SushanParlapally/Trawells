#!/usr/bin/env pwsh

Write-Host "Production Security Verification" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

$foundIssues = $false

# Check Program.cs for connection string logging
Write-Host "`nChecking Program.cs for connection string logging..." -ForegroundColor Yellow
$programPath = Join-Path (Join-Path $PSScriptRoot "..") "Program.cs"
if (Test-Path $programPath) {
    $programContent = Get-Content $programPath -Raw
    if ($programContent -match "Console\.WriteLine.*connection.*:") {
        Write-Host "   Connection string logging found in Program.cs" -ForegroundColor Red
        $foundIssues = $true
    } else {
        Write-Host "   No connection string logging in Program.cs" -ForegroundColor Green
    }
}

# Check LoginController.cs for JWT key from environment
Write-Host "`nChecking LoginController.cs for JWT environment variable..." -ForegroundColor Yellow
$loginControllerPath = Join-Path (Join-Path (Join-Path $PSScriptRoot "..") "Controllers") "LoginController.cs"
if (Test-Path $loginControllerPath) {
    $loginContent = Get-Content $loginControllerPath -Raw
    if ($loginContent -match "Environment\.GetEnvironmentVariable") {
        Write-Host "   JWT key read from environment in LoginController" -ForegroundColor Green
    } else {
        Write-Host "   JWT key not read from environment in LoginController" -ForegroundColor Red
        $foundIssues = $true
    }
}

# Summary
Write-Host "`nSecurity Verification Summary" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan

if (-not $foundIssues) {
    Write-Host "All security checks passed!" -ForegroundColor Green
} else {
    Write-Host "Some security issues found. Please review the output above." -ForegroundColor Red
}

Write-Host "`nNext Steps:" -ForegroundColor Cyan
Write-Host "1. Redeploy both frontend and backend applications" -ForegroundColor White
Write-Host "2. Monitor logs to ensure no sensitive data is exposed" -ForegroundColor White
Write-Host "3. Test login functionality to ensure JWT tokens work correctly" -ForegroundColor White

Write-Host "`nSecurity verification complete!" -ForegroundColor Green
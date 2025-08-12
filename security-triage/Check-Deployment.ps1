# Simple Deployment Check Script

Write-Host "DEPLOYMENT VERIFICATION" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host ""

# Test Backend
Write-Host "Testing Backend..." -ForegroundColor Yellow
try {
    $backendResponse = Invoke-WebRequest -Uri "https://trawells.onrender.com" -TimeoutSec 20 -UseBasicParsing -ErrorAction Stop
    Write-Host "Backend Status: UP (Status: $($backendResponse.StatusCode))" -ForegroundColor Green
    $backendUp = $true
}
catch {
    Write-Host "Backend Status: DOWN or unreachable" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    $backendUp = $false
}

Write-Host ""

# Test Frontend
Write-Host "Testing Frontend..." -ForegroundColor Yellow
try {
    $frontendResponse = Invoke-WebRequest -Uri "https://trawells.netlify.app" -TimeoutSec 20 -UseBasicParsing -ErrorAction Stop
    Write-Host "Frontend Status: UP (Status: $($frontendResponse.StatusCode))" -ForegroundColor Green
    $frontendUp = $true
}
catch {
    Write-Host "Frontend Status: DOWN or unreachable" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    $frontendUp = $false
}

Write-Host ""

# Check Environment Variables Configuration
Write-Host "Checking Environment Variables Configuration..." -ForegroundColor Yellow
if (Test-Path "appsettings.json") {
    $content = Get-Content "appsettings.json" -Raw
    if ($content -like "*PLACEHOLDER*") {
        Write-Host "Environment Variables: CONFIGURED (using placeholders)" -ForegroundColor Green
    } else {
        Write-Host "Environment Variables: WARNING (may contain secrets)" -ForegroundColor Yellow
    }
} else {
    Write-Host "Environment Variables: appsettings.json not found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "=======" -ForegroundColor Cyan

if ($backendUp -and $frontendUp) {
    Write-Host "DEPLOYMENT SUCCESSFUL!" -ForegroundColor Green
    Write-Host "Both frontend and backend are running" -ForegroundColor Green
    Write-Host ""
    Write-Host "Application URLs:" -ForegroundColor Cyan
    Write-Host "Frontend: https://trawells.netlify.app" -ForegroundColor White
    Write-Host "Backend:  https://trawells.onrender.com" -ForegroundColor White
} elseif ($backendUp) {
    Write-Host "PARTIAL SUCCESS" -ForegroundColor Yellow
    Write-Host "Backend is up, but frontend may have issues" -ForegroundColor Yellow
} elseif ($frontendUp) {
    Write-Host "PARTIAL SUCCESS" -ForegroundColor Yellow
    Write-Host "Frontend is up, but backend may have issues" -ForegroundColor Yellow
    Write-Host "Check Render.io environment variables" -ForegroundColor Yellow
} else {
    Write-Host "DEPLOYMENT ISSUES" -ForegroundColor Red
    Write-Host "Both services appear to be down" -ForegroundColor Red
}

Write-Host ""
Write-Host "Check complete." -ForegroundColor Cyan
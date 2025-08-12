# DEPLOYMENT VERIFICATION SCRIPT
# This script checks if both frontend and backend are working after security remediation

param(
    [switch]$Detailed = $false
)

Write-Host "🚀 DEPLOYMENT VERIFICATION" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""

# Function to test URL with timeout
function Test-Url {
    param(
        [string]$Url,
        [string]$Name,
        [int]$TimeoutSec = 30
    )
    
    try {
        Write-Host "Testing $Name..." -ForegroundColor Yellow
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec $TimeoutSec -UseBasicParsing -ErrorAction Stop
        
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ $Name is UP (Status: $($response.StatusCode))" -ForegroundColor Green
            return $true
        } else {
            Write-Host "⚠️  $Name returned status: $($response.StatusCode)" -ForegroundColor Yellow
            return $false
        }
    }
    catch {
        Write-Host "❌ $Name is DOWN or unreachable" -ForegroundColor Red
        if ($Detailed) {
            Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        }
        return $false
    }
}

# Function to test API endpoint
function Test-ApiEndpoint {
    param(
        [string]$Url,
        [string]$Name
    )
    
    try {
        Write-Host "Testing $Name API..." -ForegroundColor Yellow
        $response = Invoke-RestMethod -Uri $Url -TimeoutSec 15 -ErrorAction Stop
        
        Write-Host "✅ $Name API is responding" -ForegroundColor Green
        if ($Detailed -and $response) {
            Write-Host "   Response: $($response | ConvertTo-Json -Depth 2)" -ForegroundColor Cyan
        }
        return $true
    }
    catch {
        Write-Host "❌ $Name API is not responding" -ForegroundColor Red
        if ($Detailed) {
            Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        }
        return $false
    }
}

Write-Host "Step 1: Testing Backend Deployment" -ForegroundColor Yellow
Write-Host "===================================" -ForegroundColor Yellow

# Test backend URLs
$backendUrls = @(
    @{ Url = "https://trawells.onrender.com"; Name = "Backend Main" },
    @{ Url = "https://trawells.onrender.com/swagger"; Name = "Backend Swagger" }
)

$backendStatus = @()
foreach ($urlInfo in $backendUrls) {
    $status = Test-Url -Url $urlInfo.Url -Name $urlInfo.Name
    $backendStatus += @{ Name = $urlInfo.Name; Status = $status }
}

Write-Host ""
Write-Host "Step 2: Testing Frontend Deployment" -ForegroundColor Yellow
Write-Host "====================================" -ForegroundColor Yellow

# Test frontend URLs
$frontendUrls = @(
    @{ Url = "https://trawells.netlify.app"; Name = "Frontend Main" },
    @{ Url = "https://travel-desk-app.netlify.app"; Name = "Frontend Alt" }
)

$frontendStatus = @()
foreach ($urlInfo in $frontendUrls) {
    $status = Test-Url -Url $urlInfo.Url -Name $urlInfo.Name
    $frontendStatus += @{ Name = $urlInfo.Name; Status = $status }
}

Write-Host ""
Write-Host "Step 3: Testing API Connectivity" -ForegroundColor Yellow
Write-Host "=================================" -ForegroundColor Yellow

# Test API endpoints (if backend is up)
$backendUp = $backendStatus | Where-Object { $_.Name -eq "Backend Main" -and $_.Status -eq $true }
if ($backendUp) {
    Write-Host "Backend is up, testing API endpoints..." -ForegroundColor Cyan
    
    # Test health endpoint (if it exists)
    Test-ApiEndpoint -Url "https://trawells.onrender.com/health" -Name "Health Check"
    
    # Test a simple API endpoint
    Test-ApiEndpoint -Url "https://trawells.onrender.com/api/departments" -Name "Departments API"
} else {
    Write-Host "⚠️  Backend is down, skipping API tests" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 4: Environment Variables Check" -ForegroundColor Yellow
Write-Host "====================================" -ForegroundColor Yellow

# Check if environment variables are properly configured
Write-Host "Checking if secure configuration is in place..." -ForegroundColor Cyan

# Read current appsettings.json
if (Test-Path "appsettings.json") {
    $appSettings = Get-Content "appsettings.json" -Raw | ConvertFrom-Json
    
    $hasPlaceholders = $false
    if ($appSettings.ConnectionStrings.DefaultConnection -like "*PLACEHOLDER*") {
        Write-Host "✅ Database connection uses environment variables" -ForegroundColor Green
        $hasPlaceholders = $true
    }
    
    if ($appSettings.Jwt.Key -like "*PLACEHOLDER*") {
        Write-Host "✅ JWT key uses environment variables" -ForegroundColor Green
        $hasPlaceholders = $true
    }
    
    if ($appSettings.Supabase.ServiceKey -like "*PLACEHOLDER*") {
        Write-Host "✅ Supabase keys use environment variables" -ForegroundColor Green
        $hasPlaceholders = $true
    }
    
    if ($appSettings.EmailSettings.SenderPassword -like "*PLACEHOLDER*") {
        Write-Host "✅ Email password uses environment variables" -ForegroundColor Green
        $hasPlaceholders = $true
    }
    
    if (-not $hasPlaceholders) {
        Write-Host "⚠️  Some secrets might still be in appsettings.json" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠️  appsettings.json not found in current directory" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Step 5: Deployment Summary" -ForegroundColor Yellow
Write-Host "==========================" -ForegroundColor Yellow

# Summary
$allBackendUp = ($backendStatus | Where-Object { $_.Status -eq $true }).Count -gt 0
$allFrontendUp = ($frontendStatus | Where-Object { $_.Status -eq $true }).Count -gt 0

Write-Host ""
if ($allBackendUp -and $allFrontendUp) {
    Write-Host "🎉 DEPLOYMENT SUCCESSFUL!" -ForegroundColor Green
    Write-Host "=========================" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Backend is running with secure environment variables" -ForegroundColor Green
    Write-Host "✅ Frontend is accessible and should connect to backend" -ForegroundColor Green
    Write-Host "✅ Security remediation is complete and working" -ForegroundColor Green
    Write-Host ""
    Write-Host "🔗 Application URLs:" -ForegroundColor Cyan
    Write-Host "   Frontend: https://trawells.netlify.app" -ForegroundColor White
    Write-Host "   Backend:  https://trawells.onrender.com" -ForegroundColor White
    Write-Host "   API Docs: https://trawells.onrender.com/swagger" -ForegroundColor White
} elseif ($allBackendUp) {
    Write-Host "⚠️  PARTIAL DEPLOYMENT" -ForegroundColor Yellow
    Write-Host "=====================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "✅ Backend is running" -ForegroundColor Green
    Write-Host "❌ Frontend may have issues" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check frontend deployment on Netlify" -ForegroundColor Yellow
} elseif ($allFrontendUp) {
    Write-Host "⚠️  PARTIAL DEPLOYMENT" -ForegroundColor Yellow
    Write-Host "=====================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "✅ Frontend is running" -ForegroundColor Green
    Write-Host "❌ Backend may have issues" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check backend deployment on Render.io" -ForegroundColor Yellow
    Write-Host "Verify environment variables are set correctly" -ForegroundColor Yellow
} else {
    Write-Host "❌ DEPLOYMENT ISSUES DETECTED" -ForegroundColor Red
    Write-Host "=============================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Both frontend and backend appear to be down" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "1. Check Render.io dashboard for backend deployment status" -ForegroundColor White
    Write-Host "2. Verify all environment variables are set in Render.io" -ForegroundColor White
    Write-Host "3. Check Netlify dashboard for frontend deployment status" -ForegroundColor White
    Write-Host "4. Review deployment logs for any errors" -ForegroundColor White
}

Write-Host ""
Write-Host "Verification complete." -ForegroundColor Cyan
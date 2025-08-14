#!/usr/bin/env pwsh

Write-Host "Testing Deployment Health" -ForegroundColor Green
Write-Host "=========================" -ForegroundColor Green

$baseUrl = "https://trawells.onrender.com"

# Test 1: Health Check
Write-Host "`n1. Testing Health Endpoint..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET -TimeoutSec 30
    Write-Host "   Health Check: OK" -ForegroundColor Green
    Write-Host "   Response: $($healthResponse | ConvertTo-Json)" -ForegroundColor Gray
} catch {
    Write-Host "   Health Check: FAILED" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Root Endpoint
Write-Host "`n2. Testing Root Endpoint..." -ForegroundColor Yellow
try {
    $rootResponse = Invoke-RestMethod -Uri "$baseUrl/" -Method GET -TimeoutSec 30
    Write-Host "   Root Endpoint: OK" -ForegroundColor Green
    Write-Host "   Response: $rootResponse" -ForegroundColor Gray
} catch {
    Write-Host "   Root Endpoint: FAILED" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Login Endpoint (OPTIONS)
Write-Host "`n3. Testing Login OPTIONS..." -ForegroundColor Yellow
try {
    $optionsResponse = Invoke-WebRequest -Uri "$baseUrl/api/Login" -Method OPTIONS -TimeoutSec 30
    Write-Host "   Login OPTIONS: OK (Status: $($optionsResponse.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "   Login OPTIONS: FAILED" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Login Endpoint (POST with test data)
Write-Host "`n4. Testing Login POST..." -ForegroundColor Yellow
try {
    $loginData = @{
        email = "work.sushanparlapally@gmail.com"
        password = "sushan@123"
    } | ConvertTo-Json

    $headers = @{
        "Content-Type" = "application/json"
    }

    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/Login" -Method POST -Body $loginData -Headers $headers -TimeoutSec 30
    Write-Host "   Login POST: OK" -ForegroundColor Green
    Write-Host "   Token received: $($loginResponse.token -ne $null)" -ForegroundColor Gray
} catch {
    Write-Host "   Login POST: FAILED" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "   Status Code: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`nDeployment Test Complete!" -ForegroundColor Green

Write-Host "`nEnvironment Variables to Check in Render.io:" -ForegroundColor Cyan
Write-Host "- JWT_KEY: Should be set" -ForegroundColor White
Write-Host "- DATABASE_URL: Should be set" -ForegroundColor White
Write-Host "- EMAIL_PASSWORD: Should be set" -ForegroundColor White
Write-Host "- SUPABASE_ANON_KEY: MISSING - Add this!" -ForegroundColor Red
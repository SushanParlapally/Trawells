#!/usr/bin/env pwsh

Write-Host "Testing Render Deployment" -ForegroundColor Green
Write-Host "=========================" -ForegroundColor Green

$baseUrl = "https://trawells.onrender.com"
$testResults = @()

# Function to test endpoint
function Test-Endpoint {
    param(
        [string]$url,
        [string]$method = "GET",
        [string]$description,
        [hashtable]$headers = @{},
        [string]$body = $null
    )
    
    Write-Host "`nTesting: $description" -ForegroundColor Yellow
    Write-Host "URL: $url" -ForegroundColor Gray
    
    try {
        $params = @{
            Uri = $url
            Method = $method
            Headers = $headers
            TimeoutSec = 30
        }
        
        if ($body) {
            $params.Body = $body
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "✅ SUCCESS: $description" -ForegroundColor Green
        
        if ($response) {
            Write-Host "Response: $($response | ConvertTo-Json -Depth 2)" -ForegroundColor Cyan
        }
        
        return @{
            Test = $description
            Status = "PASS"
            Response = $response
        }
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $errorMessage = $_.Exception.Message
        
        if ($statusCode) {
            Write-Host "❌ FAILED: $description (HTTP $statusCode)" -ForegroundColor Red
        } else {
            Write-Host "❌ FAILED: $description" -ForegroundColor Red
        }
        Write-Host "Error: $errorMessage" -ForegroundColor Red
        
        return @{
            Test = $description
            Status = "FAIL"
            Error = $errorMessage
            StatusCode = $statusCode
        }
    }
}

# Test 1: Health Check
$testResults += Test-Endpoint -url "$baseUrl/" -description "Root Health Check"

# Test 2: Health Endpoint
$testResults += Test-Endpoint -url "$baseUrl/health" -description "Health Endpoint"

# Test 3: Swagger Documentation
$testResults += Test-Endpoint -url "$baseUrl/swagger" -description "Swagger UI"

# Test 4: API Base Path
$testResults += Test-Endpoint -url "$baseUrl/api" -description "API Base Path"

# Test 5: Login Endpoint (OPTIONS - CORS preflight)
$testResults += Test-Endpoint -url "$baseUrl/api/Login" -method "OPTIONS" -description "Login CORS Preflight"

# Test 6: Login Endpoint (POST with test credentials)
$loginBody = @{
    email = "work.sushanparlapally@gmail.com"
    password = "sushan@123"
} | ConvertTo-Json

$testResults += Test-Endpoint -url "$baseUrl/api/Login" -method "POST" -body $loginBody -description "Login with Test Credentials"

# Test 7: Database Connection Test (via login attempt)
Write-Host "`nTesting Database Connection..." -ForegroundColor Yellow
$dbTestResult = Test-Endpoint -url "$baseUrl/api/Login" -method "POST" -body $loginBody -description "Database Connection Test"
if ($dbTestResult.Status -eq "PASS" -or ($dbTestResult.StatusCode -eq 401)) {
    Write-Host "✅ Database connection is working (login endpoint responded)" -ForegroundColor Green
} else {
    Write-Host "❌ Database connection may have issues" -ForegroundColor Red
}

# Summary
Write-Host "`n" + "="*50 -ForegroundColor Cyan
Write-Host "DEPLOYMENT TEST SUMMARY" -ForegroundColor Cyan
Write-Host "="*50 -ForegroundColor Cyan

$passCount = ($testResults | Where-Object { $_.Status -eq "PASS" }).Count
$failCount = ($testResults | Where-Object { $_.Status -eq "FAIL" }).Count
$totalTests = $testResults.Count

Write-Host "`nResults: $passCount PASSED, $failCount FAILED out of $totalTests tests" -ForegroundColor White

foreach ($result in $testResults) {
    $color = if ($result.Status -eq "PASS") { "Green" } else { "Red" }
    $status = if ($result.Status -eq "PASS") { "✅" } else { "❌" }
    Write-Host "$status $($result.Test)" -ForegroundColor $color
}

# Environment Check
Write-Host "`nEnvironment Variables Check:" -ForegroundColor Cyan
Write-Host "The following environment variables should be set in Render.io:" -ForegroundColor Yellow
Write-Host "- JWT_KEY" -ForegroundColor White
Write-Host "- DATABASE_URL" -ForegroundColor White
Write-Host "- EMAIL_PASSWORD" -ForegroundColor White
Write-Host "- SUPABASE_ANON_KEY" -ForegroundColor White

# Recommendations
Write-Host "`nRecommendations:" -ForegroundColor Cyan
if ($failCount -eq 0) {
    Write-Host "🎉 All tests passed! Your Render deployment is working correctly." -ForegroundColor Green
} else {
    Write-Host "⚠️  Some tests failed. Check the errors above and:" -ForegroundColor Yellow
    Write-Host "1. Verify all environment variables are set in Render.io" -ForegroundColor White
    Write-Host "2. Check Render.io deployment logs for errors" -ForegroundColor White
    Write-Host "3. Ensure the database is accessible" -ForegroundColor White
}

Write-Host "`nDeployment test complete!" -ForegroundColor Green
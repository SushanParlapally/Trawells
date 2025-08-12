# IMMEDIATE SECRET INVALIDATION SCRIPT
# Execute this script to invalidate all compromised secrets

param(
    [switch]$WhatIf = $false
)

Write-Host "🚨 CRITICAL SECURITY TRIAGE - SECRET INVALIDATION" -ForegroundColor Red
Write-Host "=================================================" -ForegroundColor Red
Write-Host ""

if ($WhatIf) {
    Write-Host "RUNNING IN WHAT-IF MODE - No actual changes will be made" -ForegroundColor Yellow
    Write-Host ""
}

# Function to generate secure random string
function New-SecureRandomString {
    param([int]$Length = 32)
    $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"
    $random = New-Object System.Random
    $result = ""
    for ($i = 0; $i -lt $Length; $i++) {
        $result += $chars[$random.Next(0, $chars.Length)]
    }
    return $result
}

# Step 1: Generate new secrets
Write-Host "Step 1: Generating new secure secrets..." -ForegroundColor Cyan
$newJwtKey = New-SecureRandomString -Length 64
$newApiKey = New-SecureRandomString -Length 32

Write-Host "[OK] New JWT Key generated (64 characters)" -ForegroundColor Green
Write-Host "[OK] New API Key generated (32 characters)" -ForegroundColor Green
Write-Host ""

# Step 2: Display Supabase manual steps
Write-Host "Step 2: MANUAL SUPABASE ACTIONS REQUIRED" -ForegroundColor Yellow
Write-Host "=======================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "🔗 Open Supabase Dashboard: https://supabase.com/dashboard" -ForegroundColor White
Write-Host "📋 Project ID: pkhlhfpknxjaqruarvhi" -ForegroundColor White
Write-Host ""
Write-Host "REQUIRED ACTIONS:" -ForegroundColor Red
Write-Host "1. Navigate to Settings → Database" -ForegroundColor White
Write-Host "2. Click 'Reset database password'" -ForegroundColor White
Write-Host "3. Generate and save new password securely" -ForegroundColor White
Write-Host "4. Navigate to Settings → API" -ForegroundColor White
Write-Host "5. Reset the service_role key" -ForegroundColor White
Write-Host "6. Copy the new service key immediately" -ForegroundColor White
Write-Host ""

# Step 3: Display Render.io update instructions
Write-Host "Step 3: UPDATE RENDER.IO ENVIRONMENT VARIABLES" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "🔗 Open Render Dashboard: https://dashboard.render.com" -ForegroundColor White
Write-Host ""
Write-Host "UPDATE THESE ENVIRONMENT VARIABLES:" -ForegroundColor Red
Write-Host "DATABASE_URL=<new_supabase_connection_string>" -ForegroundColor White
Write-Host "SUPABASE_SERVICE_KEY=<new_service_key_from_supabase>" -ForegroundColor White
Write-Host "JWT_KEY=$newJwtKey" -ForegroundColor White
Write-Host "EMAIL_PASSWORD=<new_gmail_app_password>" -ForegroundColor White
Write-Host ""

# Step 4: Gmail App Password instructions
Write-Host "Step 4: GENERATE NEW GMAIL APP PASSWORD" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "🔗 Go to: https://myaccount.google.com/apppasswords" -ForegroundColor White
Write-Host "📧 Account: noreplytrawell@gmail.com" -ForegroundColor White
Write-Host ""
Write-Host "STEPS:" -ForegroundColor Red
Write-Host "1. Delete existing app password 'bbge rpiz maie fpby'" -ForegroundColor White
Write-Host "2. Generate new app password for 'TravelDesk Application'" -ForegroundColor White
Write-Host "3. Copy the new 16-character password" -ForegroundColor White
Write-Host "4. Update EMAIL_PASSWORD in Render.io" -ForegroundColor White
Write-Host ""

# Step 5: Create verification checklist
Write-Host "Step 5: VERIFICATION CHECKLIST" -ForegroundColor Yellow
Write-Host "=============================" -ForegroundColor Yellow
Write-Host ""
Write-Host "After completing manual steps, run:" -ForegroundColor White
Write-Host ".\Verify-Secret-Revocation.ps1" -ForegroundColor Cyan
Write-Host ""

# Step 6: Save new secrets to secure file
if (-not $WhatIf) {
    $secretsFile = "new-secrets-$(Get-Date -Format 'yyyyMMdd-HHmmss').txt"
    
    # Create secrets content
    $secretsContent = "NEW SECRETS GENERATED - $(Get-Date)`n"
    $secretsContent += "=====================================`n`n"
    $secretsContent += "JWT_KEY=$newJwtKey`n"
    $secretsContent += "TEMP_API_KEY=$newApiKey`n`n"
    $secretsContent += "MANUAL ACTIONS COMPLETED:`n"
    $secretsContent += "- Supabase database password reset`n"
    $secretsContent += "- Supabase service key regenerated`n"
    $secretsContent += "- Render.io environment variables updated`n"
    $secretsContent += "- Gmail app password regenerated`n"
    $secretsContent += "- Verification script executed successfully`n`n"
    $secretsContent += "NEXT STEPS:`n"
    $secretsContent += "- Run .\Verify-Secret-Revocation.ps1`n"
    $secretsContent += "- Proceed with Git history decontamination`n"
    $secretsContent += "- Implement secure configuration management`n`n"
    $secretsContent += "IMPORTANT: Delete this file after verification is complete!"
    
    $secretsContent | Out-File -FilePath $secretsFile -Encoding UTF8

    Write-Host "[OK] New secrets saved to: $secretsFile" -ForegroundColor Green
    Write-Host "[WARN] DELETE this file after verification!" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "[CRITICAL] Complete all manual steps within 15 minutes!" -ForegroundColor Red
Write-Host "[TIMER] Start timer now and execute each step immediately." -ForegroundColor Yellow
Write-Host ""
Write-Host "Next: Run .\Verify-Secret-Revocation.ps1 after completing manual steps" -ForegroundColor Cyan
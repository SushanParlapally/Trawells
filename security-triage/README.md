# Security Triage Scripts

## 🚨 CRITICAL: Execute Immediately Upon Secret Exposure

This directory contains scripts and documentation for immediate response to the security incident where sensitive credentials were exposed in the public GitHub repository.

## Quick Start

### Option 1: Automated Execution (Recommended)
```powershell
cd TravelDesk-master/security-triage
.\Execute-Immediate-Triage.ps1
```

### Option 2: Manual Step-by-Step
```powershell
# 1. Generate new secrets and get instructions
.\Invalidate-Secrets.ps1

# 2. Complete manual actions (Supabase, Render.io, Gmail)
# (Follow the displayed instructions)

# 3. Verify all secrets have been invalidated
.\Verify-Secret-Revocation.ps1
```

## Files in This Directory

### Scripts
- **`Execute-Immediate-Triage.ps1`** - Master orchestration script
- **`Invalidate-Secrets.ps1`** - Generates new secrets and provides invalidation instructions
- **`Verify-Secret-Revocation.ps1`** - Verifies old secrets are no longer active

### Documentation
- **`IMMEDIATE_TRIAGE_GUIDE.md`** - Complete step-by-step triage guide
- **`Supabase-Dashboard-Navigation.md`** - Detailed Supabase dashboard instructions
- **`README.md`** - This file

## Exposed Secrets (INVALIDATE IMMEDIATELY)

The following secrets were exposed in `appsettings.json`:

1. **Database Password**: `Mahendra@07`
2. **Supabase Service Key**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBraGxoZnBrbnhqYXFydWFydmhpIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1NDQ2NDE2MywiZXhwIjoyMDcwMDQwMTYzfQ.iFEKiBkz3qgFoEtdrl-ZJcr3EleJkc32HGuXceTlW6k`
3. **JWT Key**: `ThisismySecretKey568369342286956369`
4. **Email Password**: `bbge rpiz maie fpby`

## Time-Critical Actions (Complete within 15 minutes)

1. **Supabase Database Password Reset**
   - Dashboard: https://supabase.com/dashboard/project/pkhlhfpknxjaqruarvhi
   - Settings → Database → Reset database password

2. **Supabase Service Key Regeneration**
   - Settings → API → Reset service_role key

3. **Production Environment Update (Render.io)**
   - Dashboard: https://dashboard.render.com
   - Update environment variables with new secrets

4. **Gmail App Password Regeneration**
   - Account: noreplytrawell@gmail.com
   - Generate new app password

## Verification Checklist

After running the scripts, verify:

- [ ] Old database password no longer works
- [ ] Old service key returns 401/403 errors
- [ ] Application starts successfully with new secrets
- [ ] Production deployment is functional
- [ ] Email service is working
- [ ] No old secrets remain in configuration files

## Script Parameters

### Execute-Immediate-Triage.ps1
```powershell
# Dry run (no actual changes)
.\Execute-Immediate-Triage.ps1 -WhatIf

# Skip confirmation prompts
.\Execute-Immediate-Triage.ps1 -SkipConfirmation
```

### Verify-Secret-Revocation.ps1
```powershell
# Provide new secrets for testing
.\Verify-Secret-Revocation.ps1 -NewServiceKey "new_key" -NewJwtKey "new_jwt" -NewEmailPassword "new_password"
```

## Troubleshooting

### PowerShell Execution Policy
If you get execution policy errors:
```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
```

### Script Not Found
Ensure you're in the correct directory:
```powershell
cd TravelDesk-master/security-triage
Get-ChildItem *.ps1
```

### Supabase Dashboard Access Issues
- Clear browser cache
- Try incognito/private browsing
- Verify you have admin access to the project

### Render.io Environment Variables
- Changes may take 2-3 minutes to deploy
- Check deployment logs for errors
- Verify variable names match exactly

## Post-Triage Actions

After completing immediate triage:

1. **Monitor logs** for 24-48 hours
2. **Proceed to Task 2**: Git history decontamination
3. **Implement Task 3**: Secure configuration management
4. **Complete remaining security hardening tasks**

## Emergency Contacts

- **Supabase Status**: https://status.supabase.com
- **Render Status**: https://status.render.com
- **GitGuardian**: Check your email for incident details

## Security Notes

- **Delete temporary files** after verification
- **Monitor application logs** for unusual activity
- **Document all actions** with timestamps
- **Notify team members** of the incident and resolution

---

**⚠️ CRITICAL: Do not commit any files from this directory to version control!**
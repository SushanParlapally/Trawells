# IMMEDIATE SECURITY TRIAGE GUIDE

## 🚨 CRITICAL: Execute Within 15 Minutes

This guide provides immediate steps to invalidate all compromised secrets exposed in the public repository.

## Exposed Secrets Identified

1. **Supabase Database Credentials**
   - User ID: `postgres.pkhlhfpknxjaqruarvhi`
   - Password: `Mahendra@07`
   - Server: `aws-0-ap-south-1.pooler.supabase.com`

2. **Supabase Service Key**
   - Key: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBraGxoZnBrbnhqYXFydWFydmhpIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc1NDQ2NDE2MywiZXhwIjoyMDcwMDQwMTYzfQ.iFEKiBkz3qgFoEtdrl-ZJcr3EleJkc32HGuXceTlW6k`

3. **JWT Signing Key**
   - Key: `ThisismySecretKey568369342286956369`

4. **Email Service Password**
   - Gmail App Password: `bbge rpiz maie fpby`

## Immediate Action Steps

### Step 1: Invalidate Supabase Credentials (HIGHEST PRIORITY)

1. **Access Supabase Dashboard**
   - Go to: https://supabase.com/dashboard
   - Login to your account
   - Select project: `pkhlhfpknxjaqruarvhi`

2. **Reset Database Password**
   - Navigate to: Settings → Database
   - Click "Reset database password"
   - Generate new secure password
   - Save the new password securely

3. **Regenerate Service Key**
   - Navigate to: Settings → API
   - Under "Project API keys" section
   - Click "Reset" next to service_role key
   - Copy the new service key immediately

### Step 2: Update Production Environment (Render.io)

1. **Access Render Dashboard**
   - Go to: https://dashboard.render.com
   - Select your TravelDesk service

2. **Update Environment Variables**
   - Go to Environment tab
   - Update these variables with new values:
     - `DATABASE_URL` (new connection string)
     - `SUPABASE_SERVICE_KEY` (new service key)
     - `JWT_KEY` (generate new 32+ character key)
     - `EMAIL_PASSWORD` (generate new Gmail app password)

### Step 3: Generate New Secrets

Run the PowerShell scripts provided in this directory:

```powershell
# Execute immediate invalidation
.\Invalidate-Secrets.ps1

# Verify invalidation was successful
.\Verify-Secret-Revocation.ps1
```

## Verification Checklist

- [ ] Supabase database password reset
- [ ] Supabase service key regenerated
- [ ] Production environment variables updated
- [ ] Old credentials no longer work (verified)
- [ ] Application still functions with new credentials
- [ ] New JWT key generated and deployed
- [ ] New email app password generated

## Next Steps

After completing immediate triage:

1. Execute Git history decontamination (Task 2)
2. Implement secure configuration management (Task 3)
3. Update application code for new security practices

## Emergency Contacts

If you encounter issues during triage:
- Check Supabase status: https://status.supabase.com
- Check Render status: https://status.render.com
- Review application logs in Render dashboard

## Time Stamps

- Incident Detected: [TIMESTAMP]
- Triage Started: [TIMESTAMP]
- Secrets Invalidated: [TIMESTAMP]
- Production Updated: [TIMESTAMP]
- Verification Complete: [TIMESTAMP]
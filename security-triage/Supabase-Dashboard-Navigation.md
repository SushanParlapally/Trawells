# Supabase Dashboard Navigation Guide

## Quick Access Information

- **Project URL**: https://pkhlhfpknxjaqruarvhi.supabase.co
- **Dashboard URL**: https://supabase.com/dashboard/project/pkhlhfpknxjaqruarvhi
- **Project ID**: `pkhlhfpknxjaqruarvhi`

## Critical Security Actions

### 1. Reset Database Password

**Navigation Path**: Dashboard → Settings → Database

**Steps**:
1. Login to [Supabase Dashboard](https://supabase.com/dashboard)
2. Select project `pkhlhfpknxjaqruarvhi`
3. Click **Settings** in the left sidebar
4. Click **Database** in the settings menu
5. Scroll to "Database Password" section
6. Click **"Reset database password"** button
7. **IMPORTANT**: Copy the new password immediately
8. Click **"Update password"** to confirm

**What This Does**:
- Invalidates the exposed password: `Mahendra@07`
- Generates a new secure random password
- Updates all internal Supabase connections
- Requires updating your application connection strings

### 2. Regenerate Service Role Key

**Navigation Path**: Dashboard → Settings → API

**Steps**:
1. From the same project dashboard
2. Click **Settings** in the left sidebar
3. Click **API** in the settings menu
4. Scroll to "Project API keys" section
5. Find the **service_role** key row
6. Click the **"Reset"** button next to service_role
7. **CRITICAL**: Copy the new service key immediately
8. Confirm the reset action

**What This Does**:
- Invalidates the exposed service key: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
- Generates a new JWT service role token
- Maintains same permissions but with new signature
- Requires updating your application configuration

### 3. Monitor Project Activity

**Navigation Path**: Dashboard → Logs

**Steps**:
1. Click **Logs** in the left sidebar
2. Monitor for any suspicious activity
3. Check for unauthorized API calls
4. Review database connection attempts

**Look For**:
- Failed authentication attempts with old credentials
- Unusual API usage patterns
- Unexpected database queries
- Geographic anomalies in access logs

### 4. Update Connection Strings

After resetting the database password, update your connection strings:

**Old Format**:
```
User Id=postgres.pkhlhfpknxjaqruarvhi;Password=Mahendra@07;Server=aws-0-ap-south-1.pooler.supabase.com;Port=6543;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Include Error Detail=true;Timeout=60;Command Timeout=60
```

**New Format**:
```
User Id=postgres.pkhlhfpknxjaqruarvhi;Password=[NEW_PASSWORD];Server=aws-0-ap-south-1.pooler.supabase.com;Port=6543;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Include Error Detail=true;Timeout=60;Command Timeout=60
```

### 5. Verify Key Regeneration

**Check Service Key Format**:
- New service keys start with: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.`
- Should be different from the exposed key
- Length should be similar (~200+ characters)
- Contains project reference in JWT payload

**Test New Keys**:
```bash
# Test API connectivity with new service key
curl -X GET 'https://pkhlhfpknxjaqruarvhi.supabase.co/rest/v1/' \
  -H "apikey: [NEW_SERVICE_KEY]" \
  -H "Authorization: Bearer [NEW_SERVICE_KEY]"
```

## Security Best Practices

### Immediate Actions
- [ ] Reset database password
- [ ] Regenerate service role key
- [ ] Update production environment variables
- [ ] Test application connectivity
- [ ] Monitor logs for suspicious activity

### Ongoing Security
- [ ] Enable database audit logging
- [ ] Set up API usage alerts
- [ ] Configure IP restrictions if possible
- [ ] Regular key rotation schedule
- [ ] Monitor for unusual access patterns

## Troubleshooting

### "Password reset failed"
- Wait 5 minutes and try again
- Check if you have admin permissions
- Contact Supabase support if persistent

### "Service key not updating"
- Clear browser cache
- Try incognito/private browsing mode
- Verify you're on the correct project

### "Application can't connect"
- Verify new connection string format
- Check environment variable updates
- Confirm new password was copied correctly
- Test connection string locally first

## Emergency Contacts

- **Supabase Status**: https://status.supabase.com
- **Supabase Support**: https://supabase.com/support
- **Documentation**: https://supabase.com/docs/guides/database/managing-passwords

## Time-Critical Reminders

⏱️ **Complete within 15 minutes of detection**
🔄 **Update production immediately after reset**
📝 **Document all changes with timestamps**
🔍 **Verify old credentials no longer work**
📊 **Monitor logs for 24-48 hours post-reset**
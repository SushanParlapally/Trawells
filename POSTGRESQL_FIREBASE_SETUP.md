# PostgreSQL & Supabase Storage Migration Setup Guide

This guide will help you set up the TravelDesk application with PostgreSQL (Supabase) and Supabase Storage for PDF files.

## 🔄 Migration Summary

- **Database**: SQL Server → PostgreSQL (Supabase)
- **File Storage**: Local/DB → Supabase Storage
- **PDF Handling**: Direct file download → Supabase download URLs

## 📋 Prerequisites

1. **Supabase PostgreSQL Database**
   - Create a Supabase account at https://supabase.com
   - Create a new project
   - Get your connection string from Project Settings → Database

2. **Supabase Storage**
   - Enable Storage in your Supabase project
   - Create a storage bucket named "pdfs"
   - Configure storage policies for public access

## 🔧 Configuration Steps

### 1. Update Connection String

Edit `appsettings.json` and replace the connection strings with your Supabase PostgreSQL details:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.your-project-ref.supabase.co;Database=postgres;Username=postgres;Password=your-database-password;SSL Mode=Require;Trust Server Certificate=true;",
    "LocalConnection": "Host=db.your-project-ref.supabase.co;Database=postgres;Username=postgres;Password=your-database-password;SSL Mode=Require;Trust Server Certificate=true;",
    "CloudConnection": "Host=db.your-project-ref.supabase.co;Database=postgres;Username=postgres;Password=your-database-password;SSL Mode=Require;Trust Server Certificate=true;"
  }
}
```

### 2. Configure Supabase Storage

1. **Update Supabase Configuration** in `appsettings.json`:

```json
{
  "Supabase": {
    "Url": "https://your-project-ref.supabase.co",
    "Key": "your-supabase-anon-key",
    "ServiceKey": "your-supabase-service-role-key"
  }
}
```

2. **Get Your Supabase Keys**:
   - Go to your Supabase project → Settings → API
   - Copy the "Project URL" and "anon public" key
   - For service operations, use the "service_role" key (keep this secret)

### 3. Database Migration

Run the following commands to set up the PostgreSQL database:

```bash
# Apply migrations to create tables
dotnet ef database update

# Verify the database connection
dotnet run
```

## 🚀 Testing the Migration

### 1. Test Database Connection

Start the application and verify:
- Database tables are created successfully
- API endpoints respond correctly
- No SQL Server-specific errors

### 2. Test Supabase Integration

1. **Create a travel request** through the API
2. **Book the ticket** (this will generate a PDF and upload to Supabase)
3. **Download the ticket** (should return a Supabase download URL)

### 3. API Endpoints to Test

- `GET /api/travel-requests` - List all requests
- `POST /api/travel-requests/{id}/book` - Book a ticket (generates PDF → Supabase)
- `GET /api/travel-requests/{id}/download-ticket` - Get Supabase download URL
- `GET /api/travel-requests/{id}/secure-download-link` - Get secure download link

## 🔍 Troubleshooting

### Database Issues

1. **Connection String Format**: Ensure SSL mode is set to "Require"
2. **Firewall**: Make sure your IP is whitelisted in Supabase
3. **Credentials**: Verify username/password are correct

### Supabase Issues

1. **API Keys**: Ensure the anon key and service role key are correct
2. **Storage Policies**: Check Supabase Storage policies for public access
3. **Project URL**: Verify the project URL matches your Supabase project

### Common Errors

- **"Supabase configuration is missing"**: Check your appsettings.json for correct Supabase URL and keys
- **"SSL connection required"**: Ensure SSL Mode=Require in connection string
- **"Invalid project URL"**: Check Supabase project configuration

## 📁 File Structure Changes

```
TravelDesk-master/
├── appsettings.json (updated with Supabase config)
├── Program.cs (updated for PostgreSQL + Supabase)
├── TravelDesk.csproj (updated dependencies)
├── Interface/ISupabaseStorageService.cs (new)
├── Service/SupabaseStorageService.cs (new)
├── Controllers/TravelAdminController.cs (updated for Supabase)
└── Migrations/ (new PostgreSQL migrations)
```

## 🔄 Rollback Instructions

If you need to rollback to SQL Server:

1. **Restore SQL Server package**:
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8" />
   ```

2. **Update Program.cs** to use SQL Server:
   ```csharp
   options.UseSqlServer(connectionString);
   ```

3. **Restore connection strings** in `appsettings.json`

4. **Delete PostgreSQL migrations** and recreate SQL Server ones:
   ```bash
   dotnet ef migrations remove
   dotnet ef migrations add InitialCreate
   ```

## ✅ Verification Checklist

- [ ] Supabase PostgreSQL connection working
- [ ] Supabase Storage configured
- [ ] Database migrations applied successfully
- [ ] PDF generation and upload to Supabase working
- [ ] Download URLs returning Supabase links
- [ ] All API endpoints responding correctly
- [ ] No SQL Server dependencies remaining

## 🆘 Support

If you encounter issues:

1. Check the application logs for detailed error messages
2. Verify all configuration values are correct
3. Test database connection separately
4. Test Firebase credentials separately
5. Ensure all required packages are installed

## 📝 Notes

- **Development**: Uses SQLite for local development
- **Production**: Uses PostgreSQL (Supabase) for production
- **PDF Storage**: All PDFs are now stored in Supabase Storage
- **Download URLs**: Return Supabase Storage URLs instead of file downloads
- **Backup**: Original SQL Server migrations are backed up in `Migrations_Backup/` 
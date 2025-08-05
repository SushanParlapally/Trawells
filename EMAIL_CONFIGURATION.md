# Email and PDF Download Configuration Guide

## 🎯 **Problem Solved**
The previous implementation used localhost URLs in emails, which wouldn't work when users accessed the system from different devices. This has been fixed with multiple solutions.

## ✅ **Solutions Implemented**

### 1. **Configuration-Based URLs**
- Uses `appsettings.json` to configure base URLs
- Works across all devices and environments
- Fallback to request-based URLs if configuration is missing

### 2. **PDF Email Attachments**
- Automatically attaches PDF tickets to booking confirmation emails
- Users get the ticket immediately without needing to click links
- Works even if the user is offline or on a different device

### 3. **Secure Download Links**
- Generates secure, time-limited download links
- Links work across all devices and networks
- 24-hour expiration for security

## 🔧 **Configuration Setup**

### **Development Environment**
Update `appsettings.Development.json`:
```json
{
  "AppSettings": {
    "BaseUrl": "https://localhost:7075",
    "ApiBaseUrl": "https://localhost:7075"
  }
}
```

### **Production Environment (No Custom Domain)**
If you don't have a custom domain, you have several options:

#### **Option 1: Use Your Computer's IP Address**
Update `appsettings.json`:
```json
{
  "AppSettings": {
    "BaseUrl": "https://192.168.1.100:7075",
    "ApiBaseUrl": "https://192.168.1.100:7075"
  }
}
```
*Replace `192.168.1.100` with your actual IP address*

#### **Option 2: Use Local Network Hostname**
Update `appsettings.json`:
```json
{
  "AppSettings": {
    "BaseUrl": "https://your-computer-name:7075",
    "ApiBaseUrl": "https://your-computer-name:7075"
  }
}
```
*Replace `your-computer-name` with your actual computer name*

#### **Option 3: Use ngrok for External Access**
1. Install ngrok: https://ngrok.com/
2. Run: `ngrok http 7075`
3. Update `appsettings.json`:
```json
{
  "AppSettings": {
    "BaseUrl": "https://abc123.ngrok.io",
    "ApiBaseUrl": "https://abc123.ngrok.io"
  }
}
```
*Replace `abc123.ngrok.io` with your actual ngrok URL*

#### **Option 4: Use Azure/AWS/Cloud Hosting**
If you deploy to cloud services:
```json
{
  "AppSettings": {
    "BaseUrl": "https://your-app.azurewebsites.net",
    "ApiBaseUrl": "https://your-app.azurewebsites.net"
  }
}
```

### **Staging Environment**
Create `appsettings.Staging.json`:
```json
{
  "AppSettings": {
    "BaseUrl": "https://staging.your-domain.com",
    "ApiBaseUrl": "https://staging.your-domain.com"
  }
}
```

## 📧 **Email Features**

### **Automatic PDF Attachment**
- When a ticket is booked, the PDF is automatically generated
- The PDF is attached to the confirmation email
- Users can download the ticket immediately from their email

### **Download Link in Email**
- Email also includes a download link as backup
- Link uses the configured base URL (not localhost)
- Works on any device or network

## 🔐 **Security Features**

### **Secure Download Links**
- Generate time-limited download links
- 24-hour expiration
- Can be used on any device
- No authentication required for public downloads

### **Public Download Endpoint**
- `/api/travel-requests/{id}/download-ticket`
- No authentication required
- Only works for booked tickets
- Can be accessed from any device

## 🚀 **How It Works**

### **1. Booking Process**
1. Travel admin books a ticket
2. System generates PDF ticket
3. System sends email with:
   - PDF attachment (immediate access)
   - Download link (backup access)
   - Both use configured base URL

### **2. Email Delivery**
- PDF is attached to email
- Download link uses production URL
- Works on any device or network
- No dependency on localhost

### **3. Download Options**
- **Option 1**: Download from email attachment (immediate)
- **Option 2**: Click download link in email
- **Option 3**: Generate new secure download link
- **Option 4**: Use public download endpoint

## 📱 **Cross-Device Compatibility**

### **Mobile Devices**
- PDF attachments work on all email apps
- Download links work on mobile browsers
- No localhost dependency

### **Desktop Computers**
- PDF attachments work on all email clients
- Download links work on all browsers
- Works with corporate firewalls

### **Tablets**
- Full compatibility with all email apps
- Download links work on tablet browsers
- Responsive design for all screen sizes

## 🔄 **Environment Setup**

### **Development**
```bash
# Use localhost for development
BaseUrl: "https://localhost:7075"
```

### **Local Network (No Custom Domain)**
```bash
# Use your computer's IP address
BaseUrl: "https://192.168.1.100:7075"

# Or use your computer name
BaseUrl: "https://your-computer-name:7075"

# Or use ngrok for external access
BaseUrl: "https://abc123.ngrok.io"
```

### **Production**
```bash
# Use production domain
BaseUrl: "https://your-production-domain.com"
```

## 🛠 **Frontend Integration**

### **Generate Secure Download Link**
```typescript
const downloadLink = await travelAdminService.generateSecureDownloadLink(travelRequestId);
// Returns: { downloadUrl, expiresAt, message }
```

### **Download PDF**
```typescript
const pdfBlob = await travelAdminService.downloadTicketPdf(travelRequestId, token);
// Downloads PDF for any device
```

### **Generate PDF for Email**
```typescript
const pdfBlob = await travelAdminService.generateTicketPdf(travelRequestId);
// Generates PDF for email attachment
```

## ✅ **Benefits**

1. **Universal Access**: Works on any device or network
2. **No Localhost Dependency**: Uses configured production URLs
3. **Immediate Access**: PDF attached to email
4. **Backup Options**: Multiple ways to download tickets
5. **Security**: Time-limited download links
6. **Reliability**: Works even if email links fail

## 🎉 **Result**
Users can now download their tickets from any device, anywhere in the world, without any localhost dependencies! 
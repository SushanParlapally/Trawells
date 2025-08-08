# 🚀 Travel Desk Management System

A comprehensive travel request management system built with **ASP.NET Core** and **React TypeScript**.

## 🌐 **Live Demo**

- **Frontend**: [https://travel-desk-app.netlify.app](https://travel-desk-app.netlify.app)
- **Backend API**: [https://trawells.onrender.com](https://trawells.onrender.com)
- **API Health Check**: [https://trawells.onrender.com/health](https://trawells.onrender.com/health)

## 🛠 **Tech Stack**

### **Backend**
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core** - ORM with SQLite/SQL Server
- **JWT Authentication** - Secure token-based auth
- **AutoMapper** - Object mapping
- **iText7** - PDF generation for travel tickets

### **Frontend**
- **React 19** with TypeScript
- **Material-UI (MUI)** - Professional UI components
- **Redux Toolkit** - State management
- **React Hook Form** - Form handling
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **Recharts** - Data visualization

### **Deployment**
- **Render** - Backend hosting
- **Netlify** - Frontend hosting
- **SQLite** - Production database
- **GitHub** - Version control

## 🎯 **Key Features**

### **Multi-Role System**
- **Employee**: Submit travel requests, track status
- **Manager**: Approve/reject requests, team management
- **Travel Admin**: Book tickets, generate PDFs, workflow management
- **Admin**: User management, system administration

### **Travel Request Workflow**
1. **Employee** submits travel request
2. **Manager** reviews and approves
3. **Travel Admin** books tickets and generates PDFs
4. **Employee** receives email with ticket download link

### **Advanced Features**
- **Real-time status tracking**
- **PDF ticket generation**
- **Email notifications**
- **Responsive design**
- **Accessibility compliance**
- **Performance monitoring**

## 🚀 **Quick Start**

### **Backend**
```bash
cd TravelDesk-master
dotnet restore
dotnet run
```

### **Frontend**
```bash
cd travel-desk-frontend
npm install
npm run dev
```

## 📊 **API Endpoints**

- `GET /health` - API status
- `POST /api/Login` - User authentication
- `GET /api/Users` - User management
- `GET /api/Department` - Department data
- `GET /api/TravelRequest` - Travel requests
- `POST /api/TravelRequest` - Create travel request
- `GET /api/TravelRequest/{id}/ticket-pdf` - Download ticket PDF

## 🎨 **UI/UX Features**

- **Modern Material Design**
- **Responsive layout** (mobile, tablet, desktop)
- **Dark/Light theme support**
- **Accessibility compliant** (WCAG 2.1)
- **Loading states and error handling**
- **Form validation**
- **Data visualization charts**

## 🔒 **Security**

- **JWT token authentication**
- **Role-based access control**
- **CORS configuration**
- **Input validation**
- **SQL injection prevention**
- **XSS protection**

## 📈 **Performance**

- **Lazy loading** for components
- **Code splitting** by routes
- **Optimized bundle size**
- **CDN for static assets**
- **Database query optimization**

## 🧪 **Testing**

- **Unit tests** with Vitest
- **Integration tests** with Playwright
- **Accessibility tests** with axe-core
- **Performance monitoring**

## 🌍 **Deployment Architecture**

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Netlify       │    │   Render        │    │   SQLite        │
│   (Frontend)    │◄──►│   (Backend)     │◄──►│   (Database)    │
│                 │    │                 │    │                 │
│ React + TypeScript│  │ ASP.NET Core    │  │ File-based DB   │
│ Material-UI     │    │ Entity Framework│    │ Auto-created    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 📱 **Mobile Responsive**

- **Progressive Web App** features
- **Touch-friendly** interface
- **Offline capability**
- **Push notifications** (ready for implementation)

## 🎯 **Business Value**

- **Streamlined travel request process**
- **Reduced manual paperwork**
- **Real-time status tracking**
- **Automated email notifications**
- **Professional ticket generation**
- **Comprehensive reporting**

## 🔧 **Development Setup**

### **Prerequisites**
- .NET 8.0 SDK
- Node.js 18+
- SQL Server (development)
- Git

### **Environment Variables**
```env
# Backend
DATABASE_URL=your_database_connection
JWT_KEY=your_jwt_secret

# Frontend
VITE_API_BASE_URL=https://trawells.onrender.com
```

## 📄 **License**

This project is developed as a portfolio piece demonstrating full-stack development skills.

---

**Built with ❤️ using modern web technologies for professional travel management.** 
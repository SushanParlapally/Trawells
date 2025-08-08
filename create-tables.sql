-- Manual table creation script for Supabase PostgreSQL
-- This script creates all the tables needed for the TravelDesk application

-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(255) NOT NULL,
    "LastName" VARCHAR(255) NOT NULL,
    "Address" VARCHAR(500) NOT NULL,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "MobileNum" VARCHAR(20) NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "RoleId" INTEGER,
    "DepartmentId" INTEGER,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Roles table
CREATE TABLE IF NOT EXISTS "Roles" (
    "Id" SERIAL PRIMARY KEY,
    "RoleName" VARCHAR(100) NOT NULL UNIQUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Departments table
CREATE TABLE IF NOT EXISTS "Departments" (
    "Id" SERIAL PRIMARY KEY,
    "DepartmentName" VARCHAR(255) NOT NULL UNIQUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Projects table
CREATE TABLE IF NOT EXISTS "Projects" (
    "Id" SERIAL PRIMARY KEY,
    "ProjectName" VARCHAR(255) NOT NULL UNIQUE,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create TravelRequests table
CREATE TABLE IF NOT EXISTS "TravelRequests" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ProjectId" INTEGER NOT NULL,
    "ReasonForTravel" TEXT NOT NULL,
    "FromLocation" VARCHAR(255) NOT NULL,
    "ToLocation" VARCHAR(255) NOT NULL,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Pending',
    "RequestDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "TravelDate" TIMESTAMP,
    "ReturnDate" TIMESTAMP,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create BookingDetails table
CREATE TABLE IF NOT EXISTS "BookingDetails" (
    "Id" SERIAL PRIMARY KEY,
    "TravelRequestId" INTEGER NOT NULL,
    "BookingType" VARCHAR(50) NOT NULL,
    "BookingReference" VARCHAR(255),
    "Amount" DECIMAL(10,2),
    "Comments" TEXT,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create EmailSettings table
CREATE TABLE IF NOT EXISTS "EmailSettings" (
    "Id" SERIAL PRIMARY KEY,
    "SmtpServer" VARCHAR(255) NOT NULL,
    "SmtpPort" INTEGER NOT NULL,
    "SenderEmail" VARCHAR(255) NOT NULL,
    "SenderPassword" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Add foreign key constraints
ALTER TABLE "Users" ADD CONSTRAINT "FK_Users_Roles" FOREIGN KEY ("RoleId") REFERENCES "Roles"("Id") ON DELETE SET NULL;
ALTER TABLE "Users" ADD CONSTRAINT "FK_Users_Departments" FOREIGN KEY ("DepartmentId") REFERENCES "Departments"("Id") ON DELETE SET NULL;
ALTER TABLE "TravelRequests" ADD CONSTRAINT "FK_TravelRequests_Users" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE;
ALTER TABLE "TravelRequests" ADD CONSTRAINT "FK_TravelRequests_Projects" FOREIGN KEY ("ProjectId") REFERENCES "Projects"("Id") ON DELETE CASCADE;
ALTER TABLE "BookingDetails" ADD CONSTRAINT "FK_BookingDetails_TravelRequests" FOREIGN KEY ("TravelRequestId") REFERENCES "TravelRequests"("Id") ON DELETE CASCADE;

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users"("Email");
CREATE INDEX IF NOT EXISTS "IX_Users_RoleId" ON "Users"("RoleId");
CREATE INDEX IF NOT EXISTS "IX_Users_DepartmentId" ON "Users"("DepartmentId");
CREATE INDEX IF NOT EXISTS "IX_TravelRequests_UserId" ON "TravelRequests"("UserId");
CREATE INDEX IF NOT EXISTS "IX_TravelRequests_ProjectId" ON "TravelRequests"("ProjectId");
CREATE INDEX IF NOT EXISTS "IX_TravelRequests_Status" ON "TravelRequests"("Status");
CREATE INDEX IF NOT EXISTS "IX_BookingDetails_TravelRequestId" ON "BookingDetails"("TravelRequestId");

-- Insert default data
INSERT INTO "Roles" ("RoleName") VALUES 
    ('Admin'),
    ('Manager'),
    ('Employee'),
    ('TravelAdmin')
ON CONFLICT ("RoleName") DO NOTHING;

INSERT INTO "Departments" ("DepartmentName") VALUES 
    ('IT'),
    ('HR'),
    ('Finance'),
    ('Marketing'),
    ('Operations')
ON CONFLICT ("DepartmentName") DO NOTHING;

INSERT INTO "Projects" ("ProjectName") VALUES 
    ('Project Alpha'),
    ('Project Beta'),
    ('Project Gamma')
ON CONFLICT ("ProjectName") DO NOTHING;

-- Insert a default admin user (password: admin123)
INSERT INTO "Users" ("FirstName", "LastName", "Address", "Email", "MobileNum", "Password", "RoleId", "DepartmentId") VALUES 
    ('Admin', 'User', '123 Admin St', 'admin@company.com', '1234567890', 'admin123', 1, 1)
ON CONFLICT ("Email") DO NOTHING; 
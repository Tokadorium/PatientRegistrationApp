-- Set database name
DECLARE @dbName NVARCHAR(128) = N'PatientRegistrationDB';

-- Check if the database exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbName)
BEGIN
    -- Create the database
    EXEC('CREATE DATABASE [' + @dbName + ']');
END
GO

-- Use the database
USE [PatientRegistrationDB];
GO

-- Create Users table if it does not exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        UserRole NVARCHAR(50) NOT NULL DEFAULT 'User',
        FailedAttempts INT NOT NULL DEFAULT 0,
        LockedUntil DATETIME NULL,
        MetaData NVARCHAR(MAX) NULL
    );
    CREATE INDEX IX_Users_Username ON Users(Username);
END
GO

-- Create Patients table if it does not exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Patients')
BEGIN
    CREATE TABLE Patients (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        PESEL NVARCHAR(20) NOT NULL UNIQUE,
        Phone NVARCHAR(50) NULL,
        Email NVARCHAR(255) NULL,
        Street NVARCHAR(255) NULL,
        BuildingNumber NVARCHAR(20) NULL,
        ApartmentNumber NVARCHAR(20) NULL,
        PostalCode NVARCHAR(20) NULL,
        City NVARCHAR(100) NULL,
        MetaData NVARCHAR(MAX) NULL
    );
    CREATE INDEX IX_Patients_PESEL ON Patients(PESEL);
END
GO
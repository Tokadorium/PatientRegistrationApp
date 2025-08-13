# PatientRegistrationApp

## Prerequisites
- Visual Studio 2022 (or compatible)
- .NET Framework 4.7.2 Developer Pack
- SQL Server (Express or higher)

## Setup Instructions

### 1. Database Setup
- Open `setup.sql` (provided in the repository in `Build` folder) in SQL Server Management Studio or another SQL tool.
- Execute the script. It will:
  - Create a database named `PatientRegistrationDB` (if it does not exist).
  - Create the required `Users` and `Patients` tables (if they do not exist).

### 2. Configure Database Connection
- Set the environment variable `DB_CONN_STR` to your SQL Server connection string.
  - Example:
    ```
    Data Source=localhost;Initial Catalog=PatientRegistrationDB;Integrated Security=True
    ```
  - On Windows, you can set this via command line:
    ```
    setx DB_CONN_STR "Data Source=localhost;Initial Catalog=PatientRegistrationDB;Integrated Security=True"
    ```

### 3. Build and Run
- Open the solution in Visual Studio 2022.
- Restore NuGet packages if prompted.
- Build the solution.
- Run the application.

### 4. Development Utilities
- On startup, a development form allows you to seed test users and patients for demo/testing purposes.

## Notes
- Default user roles: `Admin`, `Manager`, `User`.
- The application will not start if the database connection string is not configured or the database is missing.
- **Default logins and passwords (after seeding via the development form):**
  - **Admin:**  
    Username: `admin`  
    Password: `admin123`
  - **Manager:**  
    Username: `manager`  
    Password: `manager123`
  - **User:**  
    Username: `user`  
    Password: `user123`
  - **All main operations (adding, editing, searching, and deleting patients) are performed via the tool strip menu in the main application window.**

## Troubleshooting
- If you can not generate patients ensure to relaunch Visual Studio again after setting `DB_CONN_STR`

---

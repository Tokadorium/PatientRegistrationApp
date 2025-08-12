using PatientRegistrationApp.BLL;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Tests
{
    public static class UserSeeder
    {
        public static void SeedDefaultUsers()
        {
            var userDAL = new UserDAL();

            var defaultUsers = new[]
            {
                new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.Hash("admin123"),
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@example.com",
                    UserRole = "Admin",
                    FailedAttempts = 0,
                    LockedUntil = null,
                    MetaData = null
                },
                new User
                {
                    Username = "manager",
                    PasswordHash = PasswordHasher.Hash("manager123"),
                    FirstName = "John",
                    LastName = "Manager",
                    Email = "manager@example.com",
                    UserRole = "Manager",
                    FailedAttempts = 0,
                    LockedUntil = null,
                    MetaData = null
                },
                new User
                {
                    Username = "user",
                    PasswordHash = PasswordHasher.Hash("user123"),
                    FirstName = "Jane",
                    LastName = "User",
                    Email = "user@example.com",
                    UserRole = "User",
                    FailedAttempts = 0,
                    LockedUntil = null,
                    MetaData = null
                }
            };

            foreach (var user in defaultUsers)
            {
                // Only create if doesn't exist
                if (userDAL.GetByUsername(user.Username) == null)
                {
                    userDAL.CreateUser(user);
                }
            }
        }
    }
}
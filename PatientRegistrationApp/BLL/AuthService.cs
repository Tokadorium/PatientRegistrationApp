using System;
using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.BLL
{
    public static class AuthService
    {
        public static User TryLogin(string username, string password, out string errorMessage)
        {
            var userDAL = new UserDAL();
            var user = userDAL.GetByUsername(username);

            errorMessage = null;

            if (user == null)
            {
                errorMessage = "Invalid credentials.";
                return null;
            }

            // Get user role after confirming user exists
            user.UserRole = userDAL.GetUserRole(user.Id);

            // Check if account is locked
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.Now)
            {
                errorMessage = $"Account is locked until {user.LockedUntil.Value}. Please try again later.";
                return null;
            }

            // Verify password
            if (PasswordHasher.Verify(password, user.PasswordHash))
            {
                // Reset failed attempts on successful login
                user.FailedAttempts = 0;
                user.LockedUntil = null;
                userDAL.UpdateUser(user);
                return user;
            }
            else
            {
                // Handle failed login attempt
                user.FailedAttempts++;
                if (user.FailedAttempts >= 5)
                {
                    user.LockedUntil = DateTime.Now.AddMinutes(15);
                    errorMessage = "Too many failed attempts. Try again later.";
                }
                userDAL.UpdateUser(user);

                errorMessage = "Invalid credentials.";
                return null;
            }
        }
    }
}

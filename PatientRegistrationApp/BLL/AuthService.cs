using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientRegistrationApp.Models;
using PatientRegistrationApp.DAL;

namespace PatientRegistrationApp.BLL
{
    public static class AuthService
    {
        public static User TryLogin(string username, string password, out string errorMessage)
        {
            var dal = new UserDAL();
            var user = dal.GetByUsername(username);

            user.UserRole = dal.GetUserRole(user.Id);

            errorMessage = null;

            if (user == null)
            {
                errorMessage = "Invalid credentials.";
                return null;
            }

            // DateTime.Now may not be the best choice but yeah, maybe later
            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.Now)
            {
                errorMessage = $"Account is locked until {user.LockedUntil.Value}. Please try again later.";
                return null;
            }

            if (PasswordHasher.Verify(password, user.PasswordHash))
            {
                // reset failed attempts on successful login
                user.FailedAttempts = 0;
                user.LockedUntil = null;
                dal.UpdateUser(user);
                return user;
            }
            else
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 5)
                {
                    user.LockedUntil = DateTime.Now.AddMinutes(15);
                    errorMessage = "Too many failed attempts. Try again later.";
                }
                dal.UpdateUser(user);

                errorMessage = "Invalid credentials.";
                return null;
            }
        }
    }
}

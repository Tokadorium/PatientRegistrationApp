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
        public static User TryLogin(string username, string password)
        {
            var userDal = new UserDAL();
            var user = userDal.GetUserByUsername(username?.Trim());

            if (user == null)
                return null;

            if (!PasswordHasher.Verify(password, user.PasswordHash))
                return null;

            return user;
        }
    }
}

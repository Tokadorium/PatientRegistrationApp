using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientRegistrationApp.Models
{
    public class User
    {
        // non null fields
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int FailedAttempts { get; set; }
        public string UserRole { get; set; }

        // nullable fields
        public DateTime? LockedUntil { get; set; }
        public string MetaData { get; set; }
    }
}

using System;

namespace PatientRegistrationApp.Models
{
    public class User
    {
        // Required fields
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        
        // Personal information
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        // Security and authorization
        public string UserRole { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        
        // Additional data
        public string MetaData { get; set; }
    }
}

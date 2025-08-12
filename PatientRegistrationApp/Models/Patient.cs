namespace PatientRegistrationApp.Models
{
    public class Patient
    {
        // Required fields
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PESEL { get; set; }

        // Contact information
        public string Phone { get; set; }
        public string Email { get; set; }

        // Address information
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        // Additional data
        public string MetaData { get; set; }
    }
}

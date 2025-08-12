using PatientRegistrationApp.DAL;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.Tests
{
    public class PatientSeeder
    {
        public static void SeedPatients(int count)
        {
            var dal = new PatientDAL();
            int year = 1980;
            int month = 1;
            int day = 1;

            for (int i = 0; i < count; i++)
            {
                // Generate unique PESEL
                string pesel = GeneratePesel(year, month, day, i);

                var patient = new Patient
                {
                    FirstName = $"TestName{i}",
                    LastName = $"TestSurname{i}",
                    PESEL = pesel,
                    Phone = $"048123456{i % 10}{(i / 10) % 10}{(i / 100) % 10}",
                    Email = $"test{i}@example.com",
                    Street = $"Test Street {i}",
                    BuildingNumber = $"{(i % 100) + 1}",
                    ApartmentNumber = $"{(i % 50) + 1}",
                    PostalCode = $"{(10 + (i % 90)):D2}-{(100 + (i % 900)):D3}",
                    City = $"TestCity{i % 100}",
                    MetaData = null
                };

                dal.AddPatient(patient);

                // Increment date for next PESEL
                day++;
                if (day > 28) { day = 1; month++; }
                if (month > 12) { month = 1; year++; }
            }
        }

        private static string GeneratePesel(int year, int month, int day, int index)
        {
            int yy = year % 100;
            int mm = month;
            int dd = day;
            string basePesel = $"{yy:D2}{mm:D2}{dd:D2}{index % 10000:D4}";
            int[] weights = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int sum = 0;
            for (int i = 0; i < 10; i++)
                sum += (basePesel[i] - '0') * weights[i];
            int control = (10 - (sum % 10)) % 10;
            return basePesel.Substring(0, 10) + control.ToString();
        }
    }
}
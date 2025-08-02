using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.DAL
{
    public class PatientDAL
    {
        public List<Patient> GetAllPatients()
        {
            var patients = new List<Patient>();

            using (var conn = Db.GetConnection())
            {
                conn.Open();

                string query = @"SELECT 
                                    Id,
                                    FirstName,
                                    LastName,
                                    PESEL,
                                    Phone,
                                    Email,
                                    Street,
                                    BuildingNumber,
                                    ApartmentNumber,
                                    PostalCode,
                                    City
                                FROM Patients";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // validate required fields
                        var id = (int)reader["Id"];
                        var firstName = reader["FirstName"] as string;
                        var lastName = reader["LastName"] as string;
                        var PESEL = reader["PESEL"] as string;

                        var isValid = firstName != null && lastName != null && PESEL != null;

                        var patient = new Patient
                        {
                            Id = id,
                            FirstName = firstName ?? "???",
                            LastName = lastName ?? "???",
                            PESEL = PESEL ?? "???",
                            Phone = reader["Phone"] as string,
                            Email = reader["Email"] as string,
                            Street = reader["Street"] as string,
                            BuildingNumber = reader["BuildingNumber"] as string,
                            ApartmentNumber = reader["ApartmentNumber"] as string,
                            PostalCode = reader["PostalCode"] as string,
                            City = reader["City"] as string,
                            HasCriticalError = !isValid
                        };

                        patients.Add(patient);
                    }
                }
            }

            return patients;
        }
    }
}

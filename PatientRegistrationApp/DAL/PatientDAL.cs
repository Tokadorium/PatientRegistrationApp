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
                        };

                        patients.Add(patient);
                    }
                }
            }

            return patients;
        }
        public Patient GetPatientById(int id)
        {
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
                            City,
                            MetaData
                        FROM Patients 
                        WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var firstName = reader["FirstName"] as string;
                            var lastName = reader["LastName"] as string;
                            var PESEL = reader["PESEL"] as string;

                            return new Patient
                            {
                                Id = (int)reader["Id"],
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
                                MetaData = reader["MetaData"] as string,
                            };
                        }
                    }
                }
            }

            // patient not found
            return null;
        }
        public bool AddPatient(Patient patient)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Patients 
                        (FirstName, LastName, PESEL, Phone, Email, Street, 
                         BuildingNumber, ApartmentNumber, PostalCode, City, MetaData)
                        VALUES 
                        (@FirstName, @LastName, @PESEL, @Phone, @Email, @Street, 
                         @BuildingNumber, @ApartmentNumber, @PostalCode, @City, @MetaData)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", patient.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", patient.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PESEL", patient.PESEL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", patient.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", patient.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Street", patient.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BuildingNumber", patient.BuildingNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApartmentNumber", patient.ApartmentNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PostalCode", patient.PostalCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", patient.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MetaData", patient.MetaData ?? (object)DBNull.Value);

                    var rowsAffected = cmd.ExecuteNonQuery();
                    
                    return rowsAffected > 0;
                }
            }
        }
        public bool UpdatePatient(Patient patient)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"UPDATE Patients SET 
                            FirstName = @FirstName,
                            LastName = @LastName,
                            PESEL = @PESEL,
                            Phone = @Phone,
                            Email = @Email,
                            Street = @Street,
                            BuildingNumber = @BuildingNumber,
                            ApartmentNumber = @ApartmentNumber,
                            PostalCode = @PostalCode,
                            City = @City,
                            MetaData = @MetaData
                        WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", patient.Id);
                    cmd.Parameters.AddWithValue("@FirstName", patient.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", patient.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PESEL", patient.PESEL ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", patient.Phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", patient.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Street", patient.Street ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BuildingNumber", patient.BuildingNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApartmentNumber", patient.ApartmentNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PostalCode", patient.PostalCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@City", patient.City ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MetaData", patient.MetaData ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public bool DeletePatient(int id)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"DELETE FROM Patients WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}

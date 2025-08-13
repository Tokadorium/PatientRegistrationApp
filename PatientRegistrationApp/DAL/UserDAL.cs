using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PatientRegistrationApp.Models;

namespace PatientRegistrationApp.DAL
{
    public class UserDAL
    {
        public User GetByUsername(string username)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT 
                                    Id,
                                    Username,
                                    PasswordHash,
                                    FirstName,
                                    LastName,
                                    Email,
                                    MetaData,
                                    FailedAttempts,
                                    LockedUntil
                                 FROM Users
                                 WHERE Username = @username";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                MetaData = reader["MetaData"] as string,
                                FailedAttempts = (int)reader["FailedAttempts"],
                                // UserRole = reader["UserRole"] as string,
                                LockedUntil = reader["LockedUntil"] == DBNull.Value ? null : (DateTime?)reader["LockedUntil"]
                            };
                        }
                    }
                }
            }

            return null;
        }
        public User GetById(int id)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT 
                                    Id,
                                    Username,
                                    PasswordHash,
                                    FirstName,
                                    LastName,
                                    Email,
                                    MetaData,
                                    FailedAttempts,
                                    LockedUntil
                                 FROM Users
                                 WHERE Id = @id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                MetaData = reader["MetaData"] as string,
                                FailedAttempts = (int)reader["FailedAttempts"],
                                // UserRole = reader["UserRole"] as string,
                                LockedUntil = reader["LockedUntil"] == DBNull.Value ? null : (DateTime?)reader["LockedUntil"]
                            };
                        }
                    }
                }
            }

            return null;
        }
        public string GetUserRole(int id)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT UserRole FROM Users WHERE Id = @id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var role = cmd.ExecuteScalar();

                    // place to use some auth service to pull the role and immediately
                    // encrypt it
                    return role?.ToString();
                }
            }
        }
        public bool CreateUser(User user)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO Users 
                        (Username, PasswordHash, FirstName, LastName, Email, MetaData, FailedAttempts, LockedUntil, UserRole)
                    VALUES 
                        (@Username, @PasswordHash, @FirstName, @LastName, @Email, @MetaData, @FailedAttempts, @LockedUntil, @UserRole)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MetaData", user.MetaData ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FailedAttempts", user.FailedAttempts);
                    cmd.Parameters.AddWithValue("@UserRole", user.UserRole ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LockedUntil", user.LockedUntil ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public bool UpdateUser(User user)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"
                    UPDATE Users SET
                        PasswordHash = @PasswordHash,
                        FirstName = @FirstName,
                        LastName = @LastName,
                        Email = @Email,
                        MetaData = @MetaData,
                        FailedAttempts = @FailedAttempts,
                        LockedUntil = @LockedUntil
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MetaData", user.MetaData ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FailedAttempts", user.FailedAttempts);
                    cmd.Parameters.AddWithValue("@LockedUntil", user.LockedUntil ?? (object)DBNull.Value);
                    // cmd.Parameters.AddWithValue("@UserRole", user.UserRole ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", user.Id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public bool DeleteUser(int userId)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"DELETE FROM Users WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        // yeah it doesnt do much in client only architecture
        // i could sort of "reverse hash" the role to make it harder to find in RAM though
        public bool UserHasPermission(int userId, string requiredRole)
        {
            // hierarchy lowest to highest
            var roleHierarchy = new List<string> { "User", "Manager", "Admin" };

            using (var conn = Db.GetConnection())
            {
                conn.Open();
                string query = @"SELECT UserRole FROM Users WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", userId);

                    var role = cmd.ExecuteScalar();
                    if (role == null)
                        return false;

                    string userRole = role.ToString();

                    int userRoleIndex = roleHierarchy.IndexOf(userRole);
                    int requiredRoleIndex = roleHierarchy.IndexOf(requiredRole);

                    // if either role is not found
                    if (userRoleIndex == -1 || requiredRoleIndex == -1)
                        return false;

                    // ok if user has equal or higher role
                    return userRoleIndex >= requiredRoleIndex;
                }
            }
        }

    }
}

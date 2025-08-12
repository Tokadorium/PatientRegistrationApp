using System;
using System.Data.SqlClient;

namespace PatientRegistrationApp.DAL
{
    public static class Db
    {
        public static SqlConnection GetConnection()
        {
            // get connection string from environment variable
            string connStr = Environment.GetEnvironmentVariable("DB_CONN_STR");

            if (string.IsNullOrEmpty(connStr))
            {
                throw new InvalidOperationException("\"DB_CONN_STR\" environment variable is not set. Please set it on your system before using the program.");
            }

            return new SqlConnection(connStr);
        }
    }
}

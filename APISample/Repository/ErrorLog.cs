using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace APISample.Repository
{
    public class ErrorLog : IErrorLog
    {
        private string _connectionString;

        public ErrorLog(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("myConnectionString");
        }

        public void Error(string message)
        {
            if(message != null)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertErrorLog", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ErrorMessage", message);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                string errorMessage = DateTime.Now.ToString() + " => " + message + "\n";
                System.IO.File.AppendAllText("Error\\log.txt", errorMessage);
            }
        }
    }
}

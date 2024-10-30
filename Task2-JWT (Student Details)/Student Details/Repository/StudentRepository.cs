using System.Data;
using System.Data.SqlClient;
using Student_Details.Model;
namespace Student_Details.Data
{
    public class StudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to fetch student details by ID
        public StudentDetails GetStudentDetailsById(int id)
        {
            StudentDetails studentDetails = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT sd.StudentID, sd.Name, sd.Age, sd.Email
                    FROM StudentDetails sd
                    INNER JOIN Student s ON sd.StudentID = s.ID
                    WHERE s.ID = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            studentDetails = new StudentDetails
                            {
                                StudentID = (int)reader["StudentID"],
                                Name = reader["Name"].ToString(),
                                Age = (int)reader["Age"],
                                Email = reader["Email"].ToString()
                            };
                        }
                    }
                }
            }
            return studentDetails;
        }
    }
}

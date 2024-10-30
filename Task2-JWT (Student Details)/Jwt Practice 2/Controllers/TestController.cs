using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Jwt_Practice_2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly string _connectionString = "Server=SYSLP745;Database=master;Trusted_Connection=True;";

        // GET: api/data
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = new List<object>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT s.ID, sd.Name, sd.Age, sd.Email FROM Student s " +
                               "JOIN StudentDetails sd ON s.ID = sd.StudentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Age = reader.GetInt32(2),
                        Email = reader.GetString(3)
                    });
                }
            }

            return Ok(students);
        }

        // GET: api/data/{id}
        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            object student = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT s.ID, sd.Name, sd.Age, sd.Email FROM Student s " +
                               "JOIN StudentDetails sd ON s.ID = sd.StudentID WHERE s.ID = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    student = new
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Age = reader.GetInt32(2),
                        Email = reader.GetString(3)
                    };
                }
            }

            if (student == null)
                return NotFound(new { message = $"Student with ID {id} not found." });

            return Ok(student);
        }

        [HttpGet("TestUrl")]
        public ActionResult TestUrl()
        {
            return Ok("Success");
        }
    }
}

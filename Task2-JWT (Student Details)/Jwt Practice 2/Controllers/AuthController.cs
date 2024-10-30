using Jwt_Practice_2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt_Practice_2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public string _connectionString = "Server=SYSLP745;Database=master;Trusted_Connection=True";

        // GET: api/student/GetAllStudents
        //[Authorize]
        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Unauthorized(new { message = "Authorization token is required." });
            }
            var students = new List<StudentDetails>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = @"
                        SELECT s.ID, sd.Name, sd.Age, sd.Email
                        FROM Student s
                        JOIN StudentDetails sd ON s.ID = sd.StudentID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                students.Add(new StudentDetails
                                {
                                    StudentID = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "N/A" : reader.GetString(reader.GetOrdinal("Name")),
                                    Age = reader.IsDBNull(reader.GetOrdinal("Age")) ? 0 : reader.GetInt32(reader.GetOrdinal("Age")),
                                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "N/A" : reader.GetString(reader.GetOrdinal("Email"))
                                });
                            }
                        }
                    }

                }
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving users. Please try again later.");
            }
            return Ok(students);
        }


        private bool ValidateJwtToken(string token)
        {
            throw new NotImplementedException();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var token = GenerateJwtToken();
            return Ok(new { token });
        }

        private string GenerateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9852sdfghjkrtyui45dsjfzhjisdb gfiadshigfdashfidashfjihasdifhajid6789"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7103/",
                audience: "https://localhost:7103/",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
            






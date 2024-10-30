using Microsoft.AspNetCore.Mvc;
using Student_Details.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Student_Details.Model;

namespace JWT_with_Student_details.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JWTStudentController : ControllerBase
    {
        private readonly StudentRepository _repository;
        private readonly IConfiguration _configuration; 
        public JWTStudentController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _repository = new StudentRepository(connectionString);
            _configuration = configuration;
        }

        // API to fetch student details by ID
        
        [HttpGet("JWTStudent")]
        public IActionResult GetStudentDetailsById(int id)
        {
            var studentDetails = _repository.GetStudentDetailsById(id);

            if (studentDetails == null)
                return NotFound("Student not found.");

            return Ok(new
            {
                studentDetails.Name,
                studentDetails.Age,
                studentDetails.Email
            });
        }
        [HttpPost("login")]
        public IActionResult Login(UserLoginDto loginDto)
        {
            // Validate user credentials (this is a placeholder, replace with actual validation logic)
            if (loginDto.Username == "test" && loginDto.Password == "password")
            {
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, loginDto.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
        }
        [Authorize]
        [HttpGet("students")]
        public IActionResult GetStudents()
        {
            // Fetch student details from the database
            var students = new List<Student>
    {
        new Student { ID = 1, Name = "John Doe", Age = 20 },
        new Student { ID = 2, Name = "Jane Doe", Age = 22 }
    };
            return Ok(students);
        }

    }

}



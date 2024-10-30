using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Details.Data;
using Student_Details.Model;

namespace Student_Details.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentRepository _repository;

        public StudentController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _repository = new StudentRepository(connectionString);
        }

        // API to fetch student details by ID
        [HttpGet("{id}")]
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

    }
}

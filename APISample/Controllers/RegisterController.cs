using APISample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Primitives;
using APISample.Repository;

namespace APISample.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _secretKey;
        private IErrorLog _ErrorLog;
        private object _logger;

        public RegisterController(IConfiguration configuration,IErrorLog errorLog)
        {
            _connectionString = configuration.GetConnectionString("myConnectionString");
            _secretKey = configuration["JwtSettings:SecretKey"];
            _ErrorLog = errorLog;

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'myConnectionString' is not configured.");
            }
        }

        // https://localhost:7198/Register/register
        [Authorize]  // Enforce token-based authentication
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
        {
            if (!IsTokenValid(Request.Headers["Authorization"]))
            {
                _ErrorLog.Error("Invalid or missing token.");
                return Unauthorized("Invalid or missing token.");
            }
            

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("dbo.SPI_RegisteredUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserID", model.UserID);
                    command.Parameters.AddWithValue("@FirstName", model.FirstName);
                    command.Parameters.AddWithValue("@LastName", model.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return CreatedAtAction(nameof(GetUserById), new { id = model.Username }, model);
        }

        // Token Validation Method
        private bool IsTokenValid(string authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return false;

            var token = authorizationHeader.Substring(7); // Extract token after "Bearer "
            var key = Encoding.UTF8.GetBytes(_secretKey);

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // https://localhost:7198/Register/GetAllUsers
        //[Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsTokenValid(Request.Headers["Authorization"]))
            {
                
                _ErrorLog.Error("Invalid or missing token.");
                return Unauthorized("Invalid or missing token.");
            }
           

            var users = new List<RegisterModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SPR_GetAllRegisteredUsers123", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new RegisterModel
                                {
                                    UserID = (int)reader["UserID"],
                                    FirstName = (string)reader["FirstName"],
                                    LastName = (string)reader["LastName"],
                                    DateOfBirth = (DateTime)reader["DateOfBirth"],
                                    Gender = (string)reader["Gender"],
                                    PhoneNumber = (string)reader["PhoneNumber"],
                                    EmailAddress = (string)reader["EmailAddress"],
                                    Address = (string)reader["Address"],
                                    Username = (string)reader["Username"],
                                    Password = (string)reader["Password"],
                                });
                            }
                        }
                    }
                }
                return Ok(users);
            }
            catch (SqlException sqlEx)
            {

                _ErrorLog.Error(sqlEx.Message);
                Log.Error(sqlEx, "SQL error while fetching all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving users. Please try again later.");
            }
            catch (Exception ex)
            {
                _ErrorLog.Error(ex.Message);
                Log.Error(ex, "Endpoint error while fetching all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool RouteEndpoint(StringValues stringValues)
        {
            throw new NotImplementedException();
        }

        // https://localhost:7198/Register/2
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            RegisterModel? user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SPR_GetRegisteredUsersbyId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new RegisterModel
                            {
                                UserID = (int)reader["UserID"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                DateOfBirth = (DateTime)reader["DateOfBirth"],
                                Gender = (string)reader["Gender"],
                                PhoneNumber = (string)reader["PhoneNumber"],
                                EmailAddress = (string)reader["EmailAddress"],
                                Address = (string)reader["Address"],
                                Username = (string)reader["Username"],
                                Password = (string)reader["Password"],
                            };
                        }
                    }
                }
            }

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // https://localhost:7198/Register/3
        [HttpPut("EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] RegisterModel model)
        {
            if (id != model.UserID)
            {
                return BadRequest("User ID mismatch.");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SPU_EditRegisteredUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserID", model.UserID);
                    command.Parameters.AddWithValue("@FirstName", model.FirstName);
                    command.Parameters.AddWithValue("@LastName", model.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@Username", model.Username);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return NoContent();
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("SPD_DeleteRegisteredUsers", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return NoContent();
        }

        // https://localhost:7198/Register/login (token generated)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username == "Ravi" && model.Password == "Ravi@123")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.Username) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token = tokenHandler.WriteToken(token) });
            }
            return Unauthorized("Invalid credentials");
        }
        public void Error(string message=null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string errorMessage = DateTime.Now.ToString() + " => " + message + "\n";
                System.IO.File.AppendAllText("Error\\log.txt", errorMessage);
            }
        }
        


    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

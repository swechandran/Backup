using APISample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Primitives;
namespace APISample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterErrorController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _secretKey;

        public RegisterErrorController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("myConnectionString");
            _secretKey = configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'myConnectionString' is not configured.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
        {
            if (!IsTokenValid(Request.Headers["Authorization"]))
            {
                Log.Warning("Unauthorized access - Token is invalid or missing.");
                return Unauthorized("Invalid or missing token.");
            }

            try
            {
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
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error while registering user: {@Model} at {LineNumber}", model, GetLineNumber(sqlEx));
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request. Please try again later.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while registering user: {@Model} at {LineNumber}", model, GetLineNumber(ex));
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private bool IsTokenValid(StringValues stringValues)
        {
            throw new NotImplementedException();
        }

        private static int GetLineNumber(Exception ex)
        {
            var lineNumber = new StackTrace(ex, true).GetFrame(0)?.GetFileLineNumber() ?? 0;
            return lineNumber;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = new List<RegisterModel>();
            if (!IsTokenValid(Request.Headers["Authorization"]))
            {
                Log.Warning("Unauthorized access - Token missing or invalid.");
                return Unauthorized("Invalid or missing token.");
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SPR_GetAllRegisteredUsers1", connection))
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
                System.IO.File.WriteAllText("Error\\log.txt", sqlEx.Message);
                Error(sqlEx);
                Log.Error(sqlEx, "SQL error while fetching all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving users. Please try again later.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while fetching all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        public void Error(Exception ex)
        {
            var lineNumber = new StackTrace(ex, true).GetFrame(0)?.GetFileLineNumber() ?? 0;
            System.IO.File.WriteAllText("Error\\log.txt",ex.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            RegisterModel? user = null;

            try
            {
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
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error while fetching user by ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the user. Please try again later.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while fetching user by ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id, [FromBody] RegisterModel model)
        {
            if (id != model.UserID)
            {
                return BadRequest("User ID mismatch.");
            }

            try
            {
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
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error while editing user: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the user. Please try again later.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while editing user: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
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
            catch (SqlException sqlEx)
            {
                Log.Error(sqlEx, "SQL error while deleting user: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the user. Please try again later.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while deleting user: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
        [Route("{*url}", Order = 999)]
        public IActionResult HandleInvalidRoutes()
        {
            Log.Warning("Invalid endpoint accessed: " + Request.Path);
            return NotFound("The endpoint does not exist.");
        }


    }
}




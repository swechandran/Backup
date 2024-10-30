using JWT_Practice.Model;
using JWT_Practice.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController() : ControllerBase
    {
        /* private readonly IConfiguration _configuration = configuration;
         private object JwtRegisteredClaimNames;

         [HttpPost("Login")]
         public IActionResult Login([FromBody] Login request)
         {
             if (ModelState.IsValid)
             {
                 var user = UserStore.Users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
                 if (user == null)
                 {
                     // Returns a 401 Unauthorized response with a custom message.
                     return Unauthorized("Invalid user credentials.");
                 }

                 var token = IssueToken(user);

                 return Ok(new { Token = token });
             }
             // If the model state is not valid, returns a 400 Bad Request response with a custom message.
             return BadRequest("Invalid Request Body");
         }

         private string IssueToken(User user)
         {

             var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

             var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

             var claims = new List<Claim>
             {

                 new Claim("Myapp_User_Id", user.Id.ToString()),

                 new Claim(ClaimTypes.NameIdentifier, user.Username),

                 new Claim(ClaimTypes.Email, user.Email),

                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
             };
             // Adds a role claim for each role associated with the user.
             user.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

             var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],
                 audience: _configuration["Jwt:Audience"],
                 claims: claims,
                 expires: DateTime.Now.AddHours(1), // Token expiration set to 1 hour from the current time.
                 signingCredentials: credentials);
             // Serializes the JWT token to a string and returns it.
             return new JwtSecurityTokenHandler().WriteToken(token);
         }
     }

     public class Login
     {
     }*/
    }
}


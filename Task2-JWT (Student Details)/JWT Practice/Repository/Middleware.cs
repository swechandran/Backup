using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace JWT_Practice.Repository
{
    public class Middleware
    {
        // HttpClient instance used to make HTTP requests.
        private static readonly HttpClient httpClient = new HttpClient();

        // Base URL for the API endpoints on the resource server.
        private static readonly string baseUrl = "https://localhost:7239/api/users";

        // The entry point of the program.
        static async Task Main(string[] args)
        {
            try
            {
                // Authenticate and get a JWT token from the auth server.
                var token = await AuthenticateAndGetToken();
                Console.WriteLine("Token received: " + token);

                // Use the token to perform CRUD operations.
                Console.WriteLine(await GetUser(token, 1));  // Get the user with ID 1.
                Console.WriteLine(await CreateUser(token, new User { Id = 4, Username = "newuser", Email = "newuser@example.com" }));
                Console.WriteLine(await UpdateUser(token, 4, new User { Id = 4, Username = "updatedUser", Email = "updateduser@example.com" }));
                Console.WriteLine(await DeleteUser(token, 4));  // Delete the user with ID 4.

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Method to authenticate by sending credentials and receiving a JWT token.
        static async Task<string> AuthenticateAndGetToken()
        {
            // URL for the login endpoint.
            var loginUrl = "https://localhost:7035/api/Users/Login";

            // Request body containing login credentials.
            var loginRequestBody = new { Username = "admin", Password = "password" };
            var requestContent = new StringContent(JsonSerializer.Serialize(loginRequestBody), Encoding.UTF8, "application/json");

            // Make a POST request to the login URL.
            var response = await httpClient.PostAsync(loginUrl, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                // If login fails, read and return the error message.
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Login failed: {errorContent}";
            }

            // Read the response content and extract the token.
            var loginResponseContent = await response.Content.ReadAsStringAsync();
            var tokenObject = JsonSerializer.Deserialize<JsonElement>(loginResponseContent);
            return tokenObject.GetProperty("Token").GetString();
        }

        // Method to get a user's data.
        static async Task<string> GetUser(string token, int userId)
        {
            // Set the Authorization header with the received JWT token.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.GetAsync(baseUrl + $"/{userId}");
            return await ProcessResponse(response, "Get User");
        }

        // Method to create a new user.
        static async Task<string> CreateUser(string token, User user)
        {
            // Set the Authorization header and make a POST request to create a new user.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PostAsync(baseUrl,
                new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
            return await ProcessResponse(response, "Create User");
        }

        // Method to update an existing user.
        static async Task<string> UpdateUser(string token, int userId, User user)
        {
            // Set the Authorization header and make a PUT request to update the user.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PutAsync(baseUrl + $"/{userId}",
                new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
            return await ProcessResponse(response, "Update User");
        }

        // Method to delete a user.
        static async Task<string> DeleteUser(string token, int userId)
        {
            // Set the Authorization header and make a DELETE request to remove the user.
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.DeleteAsync(baseUrl + $"/{userId}");
            return await ProcessResponse(response, "Delete User");
        }

        // Helper method to process and format the response from HTTP requests.
        private static async Task<string> ProcessResponse(HttpResponseMessage response, string action)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return $"{action} succeeded: {responseBody}";
            }

            // Handle different HTTP status codes and customize the error message.
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return $"{action} Failed: Unauthorized - Token may be invalid or expired";
                case HttpStatusCode.Forbidden:
                    return $"{action} Failed: Forbidden - Insufficient permissions";
                default:
                    return $"{action} Failed: {response.StatusCode} - {responseBody}";
            }
        }
    }

    // Definition of the User model.
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public object Roles { get; internal set; }
    }
}
    



        
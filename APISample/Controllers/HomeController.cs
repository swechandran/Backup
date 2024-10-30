using APISample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Serilog;

namespace APISample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Example action to demonstrate logging
        public IActionResult SomeAction()
        {
            try
            {
                // Simulating some code that could throw an exception
                throw new InvalidOperationException("An error occurred in SomeAction.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in {Action} at {LineNumber}", nameof(SomeAction), GetLineNumber(ex));
                return StatusCode(500, "Internal server error.");
            }
        }

        // Helper method to get the line number from the exception
        private int GetLineNumber(Exception ex)
        {
            var lineNumber = new StackTrace(ex, true).GetFrame(0)?.GetFileLineNumber() ?? 0;
            return lineNumber;
        }
    }
}
// Assuming you are using ILogger for logging
/*private readonly ILogger<YourController> _logger;

public YourController(ILogger<YourController> logger)
{
    _logger = logger;
}

[HttpPost]
public IActionResult YourApiMethod()
{
    if (!RouteEndpoint(Request.Headers["Authorization"]))
    {
        string errorMessage = "Invalid or missing endpoint. Authorization header: "
                              + Request.Headers["Authorization"].ToString();

        // Log the error with additional context
        _logger.LogError(errorMessage);

        // Return Unauthorized response
        return Unauthorized(errorMessage);
    }

    // Your normal logic here if the endpoint is valid
    return Ok("Success");
}

// Dummy method to simulate RouteEndpoint validation
private bool RouteEndpoint(string authorizationHeader)
{
    // Replace with your actual endpoint logic
    return !string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ");
}
*/
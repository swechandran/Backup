using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private Timer _timer;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Service started at: {time}", DateTimeOffset.Now);

        // Run every 1 minute (60,000 ms)
        _timer = new Timer(FetchAndLogStudentDetails, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    private void FetchAndLogStudentDetails(object state)
    {
        string connectionString = _configuration.GetConnectionString("MyConnection");

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Students", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    int age = Convert.ToInt32(reader["Age"]);

                    // Log each student's details
                    LogToFile($"Student: {name}, Age: {age}");

                    // Special condition: Log students below 18
                    if (age < 18)
                    {
                        LogToFile($"Special Student Alert: {name} is under 18!");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogToFile($"Error: {ex.Message}");
        }
    }

    private void LogToFile(string message)
    {
        string path = @"C:\VSCode Programs\TestProgram\Task3-WindowService-automatic fetch Stu.Detail\StudentServiceLog.txt";

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // Write log entries to the file
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Service stopped at: {time}", DateTimeOffset.Now);
        _timer?.Dispose();
        return Task.CompletedTask;
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger services with metadata configuration.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Image to PDF API",
        Version = "v1",
        Description = "A simple API to convert images to PDF."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Image to PDF API v1");
    });
}

// Handle HTTPS redirection.
app.UseHttpsRedirection();

// Add exception handling middleware to catch unhandled errors globally.
app.UseExceptionHandler("/error");

// Enable authorization middleware (if required).
app.UseAuthorization();

// Map controller routes.
app.MapControllers();

// Run the app.
app.Run();

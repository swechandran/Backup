using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://localhost:7103/",
        ValidAudience = "https://localhost:7103/",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("iuytredfghjknb nmkj258/"))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "JWT Validation",
        Description = "Valid",
        Type = SecuritySchemeType.Http
    });
    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //            {
    //                    {
    //                        new OpenApiSecurityScheme
    //                        {
    //                            Reference = new OpenApiReference
    //                            {
    //                                Id = "Bearer",
    //                                Type = ReferenceType.SecurityScheme
    //                            }
    //                        },
    //                        new List<string>()
    //                    }
    //            });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Text;
using Backend.Data;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionString"];
var validIssuer = builder.Configuration["ValidIssuer"];
var validAudience = builder.Configuration["ValidAudience"];
var issuerSigningKey = builder.Configuration["JwtSecretKey"];
var frontendUrl = builder.Configuration["FrontendUrl"];

AddServices();
AddDbContext();
AddAuthentication();
AddCors();

var app = builder.Build();
RunMigration();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
app.UseAuthentication();



void AddServices()
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

void AddDbContext()
{
    builder.Services.AddDbContext<LingoLiftContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
    builder.Services.AddDbContext<UsersContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
}

void AddAuthentication()
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(issuerSigningKey)
                ),
            };
        });
}

void RunMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var solarDb = scope.ServiceProvider.GetRequiredService<LingoLiftContext>();
        var usersDb = scope.ServiceProvider.GetRequiredService<UsersContext>();
        if (solarDb.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            if (solarDb.Database.GetPendingMigrations().Any())
            {
                solarDb.Database.Migrate();
            }
        }
                
        if (usersDb.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            if (usersDb.Database.GetPendingMigrations().Any())
            {
                usersDb.Database.Migrate();
            }
        }
    } 
}

void AddCors()
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend",
            builder => builder
                .WithOrigins(frontendUrl)
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}
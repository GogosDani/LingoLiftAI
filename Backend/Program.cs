using System.Text;
using System.Text.Json.Serialization;
using Backend.Data;
using Backend.Models;
using Backend.Services;
using Backend.Services.AIServices;
using Backend.Services.Repositories;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

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
AddIdentity();
ConfigureSwagger();

var app = builder.Build();
RunMigration();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using var scope = app.Services.CreateScope();
var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
authenticationSeeder.AddRoles();
authenticationSeeder.AddAdmin();

app.UseCors("AllowFrontend");
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
    builder.Services.AddScoped<AuthenticationSeeder>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAIClient, AIClient>();
    builder.Services.AddScoped<IUserLanguageRepository, UserLanguageRepository>();
    builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
    builder.Services.AddScoped<ILevelRepository, LevelRepository>();
    builder.Services.AddScoped<IWordsetRepository, WordsetRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IAiWordSetRepository, AiWordsetRepository>();
    builder.Services.AddScoped<ITopicRepository, TopicRepository>();
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = null;
    });

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
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var cookie = context.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        context.Token = cookie;
                    }
                    return Task.CompletedTask;
                }
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
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

void AddIdentity()
{
    builder.Services
        .AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<UsersContext>();
}

void ConfigureSwagger()
{
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
}
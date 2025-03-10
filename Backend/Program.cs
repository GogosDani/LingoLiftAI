using dotenv.net;

var builder = WebApplication.CreateBuilder(args);
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionString"];
var validIssuer = builder.Configuration["ValidIssuer"];
var validAudience = builder.Configuration["ValidAudience"];
var issuerSigningKey = builder.Configuration["JwtSecretKey"];
var frontendUrl = builder.Configuration["FrontendUrl"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
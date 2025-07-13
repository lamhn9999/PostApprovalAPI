using Microsoft.EntityFrameworkCore;
using PostApprovalAPI.Data;
using PostApprovalAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection
builder.Services.AddDbContext<PostDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PostService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
var endpoints = app.Services.GetRequiredService<EndpointDataSource>();

foreach (var endpoint in endpoints.Endpoints)
{
    Console.WriteLine(endpoint.DisplayName);
}

app.MapControllers();
app.Run();
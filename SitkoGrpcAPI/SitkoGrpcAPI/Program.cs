using Microsoft.EntityFrameworkCore;
using SitkoGrpcAPI.Data;
using SitkoGrpcAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TodoDB");
// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<TodoDbContext>
    (options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<TodoApiService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
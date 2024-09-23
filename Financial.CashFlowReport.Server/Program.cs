using Financeiro.CashFlowReport.DataModel.Data;
using Financial.CashFlowReport.Server.Services;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddJsonFile("ConfigServer/appsettings.json", optional: false, reloadOnChange: true) 
    .AddJsonFile($"ConfigServer/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)  
    .AddEnvironmentVariables(); 

// Configuração do DbContext com PostgreSQL
builder.Services.AddDbContext<LancamentoAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));


// Configura a conexão com o MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(mongoConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var mongoDatabaseName = builder.Configuration["MongoDbSettings:DatabaseName"];
    return mongoClient.GetDatabase(mongoDatabaseName);
});

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ReportService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

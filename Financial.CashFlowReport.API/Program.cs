using Financial.CashFlowReport.Business;
using Financial.CashFlowReport.Business.CommandHandlers;
using Financial.CashFlowReport.Sdk.Extensions;
using Financial.CashFlowReport.Server.Services;
using MongoDB.Driver;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())  
    .AddJsonFile("Configs/appsettings.json", optional: false, reloadOnChange: true)  
    .AddJsonFile($"Configs/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)  
    .AddEnvironmentVariables();  



builder.Services.AddControllers();

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

// Configuração do MediatR para os Command Handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(ConsolidacaoDiariaCommandHandler).Assembly
));

// Registra o RabbitMQConsumer
builder.Services.AddSingleton<RabbitMQConsumer>();

// **Registrar o ConsolidacaoDiariaCommandHandler no container**
builder.Services.AddScoped<ConsolidacaoDiariaCommandHandler>();

builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ConnectionFactory()
    {
        HostName = configuration["RabbitMQ:HostName"],
        UserName = configuration["RabbitMQ:UserName"],
        Password = configuration["RabbitMQ:Password"]
    };
});

builder.Services.AddSingleton<RabbitMQConsumer>();

// Configuração do gRPC SDK e serviços
builder.Services.AddGrpcSdk();  
builder.Services.AddGrpc();   

// Configuração de Swagger para OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure o pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
    builder.WithOrigins("http://localhost:3000")
           .AllowAnyMethod()
           .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapear controladores (APIs HTTP)
app.MapControllers();

// Mapear serviços gRPC
app.MapGrpcService<ReportService>();

// **Inicia o consumo de mensagens RabbitMQ ao iniciar a aplicação**
using (var scope = app.Services.CreateScope())
{
    var handler = scope.ServiceProvider.GetRequiredService<ConsolidacaoDiariaCommandHandler>();
    handler.StartListeningForMessages();  // Inicia o consumo de mensagens RabbitMQ
}

app.Run();

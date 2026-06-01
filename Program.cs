using LubeAnalyst.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using LubeAnalyst;
using Azure.Storage.Blobs;
using LubeAnalyst.Repository;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var con = builder.Configuration.GetConnectionString("lubeAnalyst");
try
{
    using var conn = new SqlConnection(con);
    //Commented to avoid open DB connection from local database
    //conn.Open();
    Console.WriteLine(" Connected successfully.");
}
catch (Exception ex)
{
    Console.WriteLine(" Connection failed: " + ex.Message);
}



builder.Services.Configure<BlobStorageSettings>(
    builder.Configuration.GetSection("AzureBlobStorage"));
builder.Services.AddSingleton(serviceProvider =>
{
    var config = serviceProvider
        .GetRequiredService<IOptions<BlobStorageSettings>>().Value;

    // Create a BlobServiceClient using the connection string
    var blobClient = new BlobServiceClient(config.ConnectionString);
    var container = blobClient.GetBlobContainerClient(config.ContainerName);
    return container;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Commented to avoid open DB connection from local database
//builder.Services.AddDbContext<LabDbContext>(options =>
//    options.UseSqlServer(con));

builder.Services.AddScoped<ICustomerCommandRepository, CustomerCommandRepository>();
builder.Services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
builder.Services.AddScoped<AddCustomerHandler>();
builder.Services.AddScoped<GetAllCustomersHandler>();

//string keyVaultName = builder.Configuration["KeyVaultName"];
//var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

//  Step 2: Add Azure Key Vault as a configuration provider
//builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

//  Step 3: Read the secret value (Service Bus connection string)
string serviceBusConnectionString = builder.Configuration["ServiceBusConnectionString"] ?? "";
var apimKey = builder.Configuration["ApimSubscriptionKey"];

// (Optional) register Service Bus client for dependency injection
builder.Services.AddSingleton(sp => new ServiceBusClient(serviceBusConnectionString));
builder.Services.AddSingleton<ApimService>();

var app = builder.Build();
app.MapGet("/", () => "HELLO AZURE");

app.Run();
/*
// Command Endpoint (Write)
app.MapPost("/customers", async (AddCustomerCommand command, AddCustomerHandler handler) =>
{
    var result = await handler.Handle(command);
    return Results.Ok(result);
});

// Query Endpoint (Read)
app.MapGet("/customers", async (GetAllCustomersHandler handler) =>
{
    var result = await handler.Handle(new GetAllCustomersQuery());
    return Results.Ok(result);
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.MapSwagger();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
*/

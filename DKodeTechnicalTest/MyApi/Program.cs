using MyApi.Repositories;
using MyApi.Services;

// Create new web app builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add controllers to  dependency injection container
builder.Services.AddControllers();

// Add API explorer services for swagger/openapi doc
builder.Services.AddEndpointsApiExplorer();

// Configure swagger generation with default settings
builder.Services.AddSwaggerGen();

// Add http client factory service
builder.Services.AddHttpClient();

// Register warehouse repository with scoped lifetime - one instance per http request
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();

// Register supplier info repository with scoped lifetime - one instance per http request
builder.Services.AddScoped<ISupplierInfoRepository, SupplierInfoRepository>();

// Register csv data service with scoped lifetime - one instance per http request
builder.Services.AddScoped<CsvDataService>();

// Register prep csv service as singleton - single instance for entire app, efficient when frequently used
builder.Services.AddSingleton<PrepCsvService>();

// Build the app host
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect all requests
app.UseHttpsRedirection();

// Enable authorization
app.UseAuthorization();

// Map controller routes to endpoints
app.MapControllers();

// Start app and listen to http requests
app.Run();

// // Register a Global Serializer
// // If there are multiple entities using Guid and you don't want to add attributes 
// // to every single file, you can register a global rule as below.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PlayBook.Service.Repositories;
using PlayBook.Service.Settings;

// Tells MongoDB to automatically treat all C# Guids as standard Bson BinaryData subtypes
// (Must be called before any database actions occur)
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);

// Define the variable as a standard local variable
// Access Configuration directly via 'builder.Configuration'
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddSingleton(serviceProvider =>
{
    // Use an explicit null check or throw a descriptive exception if the section is missing
    var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()
        ?? throw new InvalidOperationException($"{nameof(MongoDbSettings)} section is missing from appsettings.json");

    var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

    // Ensure serviceSettings (from higher up in Program.cs) isn't null either
    if (serviceSettings == null)
    {
        throw new InvalidOperationException($"{nameof(ServiceSettings)} must be initialized before registering MongoDB.");
    }

    return mongoClient.GetDatabase(serviceSettings.ServiceName);
});

builder.Services.AddSingleton<IItemsRepository, ItemsRepository>();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});


builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Mounts the classic visual Swagger dashboard wrapper right on top of it
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "PlayBook API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Register a Global Serializer
// If there are multiple entities using Guid and you don't want to add attributes 
// to every single file, you can register a global rule as below.
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

// Tell MongoDB to automatically treat all C# Guids as standard Bson BinaryData subtypes
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// 1. Register modern native OpenAPI document generation
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
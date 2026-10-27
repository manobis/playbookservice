using MongoDB.Driver;
using PlayBook.Service.Entities;
using PlayBook.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo()
                .AddMongoRepository<Item>("items");

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

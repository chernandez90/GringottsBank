using GringottsBankAPI.Models;
using GringottsBankAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseInMemoryDatabase("GringottsBankDb"));
// Add services to the container.
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    // Optional: Ignore null values to reduce payload size
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));


var app = builder.Build();

// Create and Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<BankDbContext>();
    dbContext.Database.EnsureCreated(); // This will apply the seed data
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Redirect root to Swagger only in dev
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });


}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

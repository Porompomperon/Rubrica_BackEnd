using Microsoft.EntityFrameworkCore;
using RubricaApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddDbContext<RubricaContext>(opt => opt.UseInMemoryDatabase("RubricaList"));
builder.Services.AddDbContext<RubricaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RubricaContext>();
    try
    {
        db.Database.CanConnect(); // restituisce true/false
        Console.WriteLine("✅ Connessione OK");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Errore: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


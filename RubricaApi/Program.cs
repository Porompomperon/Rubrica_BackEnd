using Microsoft.EntityFrameworkCore;
using RubricaApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddDbContext<RubricaContext>(opt => opt.UseInMemoryDatabase("RubricaList"));
builder.Services.AddDbContext<RubricaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//per consentire le richieste da Angular, che è in esecuzione su un dominio diverso (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("LanPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//per consentire le richieste da Angular, che è in esecuzione su un dominio diverso (CORS)
app.UseCors("LanPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();


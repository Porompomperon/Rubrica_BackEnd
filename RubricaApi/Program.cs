using Microsoft.EntityFrameworkCore;
using RubricaApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//configurazione JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(
    Convert.FromBase64String(jwtConfig["Key"]!)
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = key,

        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization();


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
            .WithOrigins("http://localhost:4200")
            //.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); //per permettere l'invio dei cookie, necessario per il refresh token
    });
});

builder.Services.AddScoped<TokenService>();

//test da eliminare
builder.Services.AddSingleton<CookieOptions>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new CookieOptions
    {
        HttpOnly = true,
        Secure = !env.IsDevelopment(), // false in dev, true in prod
        SameSite = SameSiteMode.None,
    };
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


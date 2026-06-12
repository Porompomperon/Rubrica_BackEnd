using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricaApi.Data;
using Microsoft.AspNetCore.Authorization;
using RubricaApi.Models;

namespace RubricaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RubricaContext db;
        private readonly TokenService tokenService;
        private readonly IConfiguration config;

        public AuthController(RubricaContext db, TokenService tokenService, IConfiguration config)
        {
            this.db = db;
            this.tokenService = tokenService;
            this.config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            if (await db.Utenti.AnyAsync(u => u.Username == request.Username))
                return Conflict(new { error = "Username già in uso" });

            var utente = new Utente
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            db.Utenti.Add(utente);
            await db.SaveChangesAsync();

            return Created("", new { message = "Utente creato" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var utente = await db.Utenti
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (utente == null || !BCrypt.Net.BCrypt.Verify(request.Password, utente.PasswordHash))
                return Unauthorized(new { error = "Credenziali non valide" });

            var accessToken = tokenService.GenerateAccessToken(utente.Username);
            var refreshToken = tokenService.GenerateRefreshToken();

            // Salva il refresh token nel DB
            utente.RefreshToken = refreshToken;
            utente.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(config["Jwt:RefreshTokenExpiresDays"]!)
            );
            await db.SaveChangesAsync();

            return Ok(new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = int.Parse(config["Jwt:AccessTokenExpiresMinutes"]!) * 60,
                refresh_token = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var utente = await db.Utenti
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

            // Token non trovato, già usato, o scaduto
            if (utente == null || utente.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized(new { error = "Refresh token non valido o scaduto" });

            var newAccessToken = tokenService.GenerateAccessToken(utente.Username);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            utente.RefreshToken = newRefreshToken;
            utente.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(config["Jwt:RefreshTokenExpiresDays"]!)
            );
            await db.SaveChangesAsync();

            return Ok(new
            {
                access_token = newAccessToken,
                token_type = "Bearer",
                expires_in = int.Parse(config["Jwt:AccessTokenExpiresMinutes"]!) * 60,
                refresh_token = newRefreshToken
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            var utente = await db.Utenti.FirstOrDefaultAsync(u => u.Username == username);

            if (utente != null)
            {
                utente.RefreshToken = null;
                utente.RefreshTokenExpiry = null;
                await db.SaveChangesAsync();
            }

            return NoContent();
        }
    }

    public record AuthRequest(string Username, string Password);
    public record RefreshRequest(string RefreshToken);
}
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Infraestructura.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InfoDynamics.Aplicacion.Abstracts;

namespace InfoDynamics.Infraestructura.Processors   
{
    public class AuthTokenProcessor: IAuthTokenProcessor
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenProcessor(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public (string token, DateTime expiresUTC) GenerateJwtToken(Usuario user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {

                new Claim(ClaimTypes.Role, user.es_manager ? "Manager" : "Empleado"),
                new Claim(JwtRegisteredClaimNames.Sub, user.no_usuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                //new Claim(ClaimTypes.Role, user.es_manager.ToString())
            };

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationTimeInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,          
                audience: _jwtOptions.Audience,     
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwtToken, expires);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiresUTC)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresUTC,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName, token, cookieOptions);
        }
    }
}
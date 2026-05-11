using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using Microsoft.AspNetCore.Identity;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class AccountService : IAccountService
    {
        private readonly IAuthTokenProcessor _tokenProcessor;
        private readonly Iusuarioservicio _usuarioService;
        private readonly IUserRepository _userRepository;

       
        public AccountService(
            IAuthTokenProcessor tokenProcessor,
            Iusuarioservicio usuarioService,
            IUserRepository userRepository)
        {
            _tokenProcessor = tokenProcessor;
            _usuarioService = usuarioService;
            _userRepository = userRepository;
        }

       
        public async Task LoginAsync(loginDto loginDto)
        {
            Usuario user = null;

            if (int.TryParse(loginDto.identificador, out int numeroDeUsuario))
            {
                user = await _usuarioService.FindByIdAsync(numeroDeUsuario);
            }
            else
            {
                user = await _usuarioService.FindByEmailAsync(loginDto.identificador);
            }

            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            bool passwordValida = BCrypt.Net.BCrypt.Verify(loginDto.contrasena, user.contrasena);
            if (!passwordValida)
                throw new Exception("Contraseña/Usuario incorrecta");

            var (jwtToken, expirationDateInUtc) = _tokenProcessor.GenerateJwtToken(user);
            var refreshToken = _tokenProcessor.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpirationDateInUtc;

            await _usuarioService.UpdateWithConcurrencyAsync(user, user.RowVersion);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "REFRESH_TOKEN", refreshToken, refreshTokenExpirationDateInUtc);
        }

        public async Task RefreshtokenAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Refresh token no proporcionado");
            }

            var user = await _userRepository.GetUserbyRefreshToken(refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token inválido o expirado");
            }

            var (jwtToken, expirationDateInUtc) = _tokenProcessor.GenerateJwtToken(user);
            var newRefreshToken = _tokenProcessor.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpirationDateInUtc;
            await _usuarioService.UpdateWithConcurrencyAsync(user, user.RowVersion);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "REFRESH_TOKEN", newRefreshToken, refreshTokenExpirationDateInUtc);
        }
    }
}
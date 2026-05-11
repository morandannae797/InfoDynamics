using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;

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
            var user = await _usuarioService.VerifyUser(
                loginDto.identificador,
                loginDto.contrasena);

            if (user == null)
                throw new UnauthorizedAccessException("Contraseña/Usuario incorrecta.");

            if (user.estado_cuenta != "Activa")
                throw new UnauthorizedAccessException("La cuenta no está activa.");

            var rowVersionOriginal = user.RowVersion;

            var (jwtToken, expirationDateInUtc) = _tokenProcessor.GenerateJwtToken(user);
            var refreshToken = _tokenProcessor.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpirationDateInUtc;

            await _usuarioService.UpdateWithConcurrencyAsync(user, rowVersionOriginal);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "ACCESS_TOKEN",
                jwtToken,
                expirationDateInUtc);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "REFRESH_TOKEN",
                refreshToken,
                refreshTokenExpirationDateInUtc);
        }

        public async Task RefreshtokenAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token no proporcionado.");

            var user = await _userRepository.GetUserbyRefreshToken(refreshToken);

            if (user == null)
                throw new UnauthorizedAccessException("Refresh token inválido.");

            if (user.RefreshTokenExpiryTime == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expirado.");

            if (user.estado_cuenta != "Activa")
                throw new UnauthorizedAccessException("La cuenta no está activa.");

            var rowVersionOriginal = user.RowVersion;

            var (jwtToken, expirationDateInUtc) = _tokenProcessor.GenerateJwtToken(user);
            var newRefreshToken = _tokenProcessor.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpirationDateInUtc;

            await _usuarioService.UpdateWithConcurrencyAsync(user, rowVersionOriginal);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "ACCESS_TOKEN",
                jwtToken,
                expirationDateInUtc);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "REFRESH_TOKEN",
                newRefreshToken,
                refreshTokenExpirationDateInUtc);
        }
    }
}
using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.CustomException;
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
            // --------------------------S1.4----------------------------------------------------
            // Campos vacíos

            if (string.IsNullOrWhiteSpace(loginDto.identificador))
            {
                throw new BadRequestException(
                    "El número de empleado es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(loginDto.contrasena))
            {
                throw new BadRequestException(
                    "La contraseña es obligatoria."
                );
            }





            // ----------------------------------------S1.1.2-----------------------------------------
            if (!loginDto.identificador.All(char.IsDigit))
            {
                throw new BadRequestException(
                    "El número de empleado debe ser numérico."
                );
            }

            // ----------------------------------------S1.1.3------------------------------------------
            if (loginDto.identificador.Length != 7)
            {
                throw new BadRequestException(
                    "El número de empleado debe tener 7 dígitos."
                );
            }





            var user = await _usuarioService.VerifyUser(
                loginDto.identificador,
                loginDto.contrasena);


            //  -----------------------------------------S1.5.-----------------------------------------
            if (user == null)
                throw new UnauthorizedException(
                    "Contraseña/Usuario incorrecta."
                );





            if (user.estado_cuenta != "Activa")
                throw new UnauthorizedException(
                    "La cuenta no está activa."
                );

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
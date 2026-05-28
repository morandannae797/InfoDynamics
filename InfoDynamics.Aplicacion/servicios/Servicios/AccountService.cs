using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using System.Collections.Concurrent;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class AccountService : IAccountService
    {
        private readonly IAuthTokenProcessor _tokenProcessor;
        private readonly Iusuarioservicio _usuarioService;
        private readonly IUserRepository _userRepository;

        // Guarda intentos user
        private static readonly ConcurrentDictionary<string, int> _failedAttempts = new();

        // Guarda el tiempo de bloqueo por usuario
        private static readonly ConcurrentDictionary<string, DateTime> _blockedUsers = new();

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


            LoginValidacion.Validar(loginDto);



            LoginIntentosValidacion.ValidarBloqueo(
                loginDto,
                _blockedUsers,
                _failedAttempts);

            var user = await _usuarioService.VerifyUser(
                loginDto.identificador,
                loginDto.contrasena);



            if (user == null)
            {
                LoginIntentosValidacion.ProcesarIntentoFallido(
                    loginDto,
                    _blockedUsers,
                    _failedAttempts);

                throw new UnauthorizedException(
                    "Contraseña/Usuario incorrecta."
                );
            }

            if (user.estado_cuenta != true)
            {
                throw new UnauthorizedException(
                    "La cuenta no está activa."
                );
            }

            // Reiniciar intentos despues de login exitoso

            _failedAttempts.TryRemove(
                loginDto.identificador,
                out _);

            _blockedUsers.TryRemove(
                loginDto.identificador,
                out _);



            var rowVersionOriginal = user.RowVersion;

            var (jwtToken, expirationDateInUtc)
                = _tokenProcessor.GenerateJwtToken(user);

            var refreshToken
                = _tokenProcessor.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc
                = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime
                = refreshTokenExpirationDateInUtc;

            await _usuarioService.UpdateWithConcurrencyAsync(
                user,
                rowVersionOriginal);

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
            {
                throw new UnauthorizedAccessException(
                    "Refresh token no proporcionado.");
            }

            var user = await _userRepository
                .GetUserbyRefreshToken(refreshToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Refresh token inválido.");
            }

            if (
                user.RefreshTokenExpiryTime == null
                ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow
            )
            {
                throw new UnauthorizedAccessException(
                    "Refresh token expirado.");
            }

            if (user.estado_cuenta != true)
            {
                throw new UnauthorizedAccessException(
                    "La cuenta no está activa.");
            }

            var rowVersionOriginal = user.RowVersion;

            var (jwtToken, expirationDateInUtc)
                = _tokenProcessor.GenerateJwtToken(user);

            var newRefreshToken
                = _tokenProcessor.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc
                = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;

            user.RefreshTokenExpiryTime
                = refreshTokenExpirationDateInUtc;

            await _usuarioService.UpdateWithConcurrencyAsync(
                user,
                rowVersionOriginal);

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



    public class LoginValidacion
    {
        public static void Validar(loginDto loginDto)
        {


            if (string.IsNullOrWhiteSpace(loginDto.identificador) || string.IsNullOrWhiteSpace(loginDto.contrasena))
            {
                throw new BadRequestException(
                    "Complete los datos faltantes.");
            }


            if (!loginDto.identificador.Contains("@") && (!loginDto.identificador.All(char.IsDigit) || loginDto.identificador.Length != 7))
            {
                throw new BadRequestException(
                    "El número de empleado debe ser numérico.");
            }



            if (!loginDto.identificador.Contains("@") && (!loginDto.identificador.All(char.IsDigit) || loginDto.identificador.Length != 7))
            {
                throw new BadRequestException(
                    "El número de empleado debe tener 7 dígitos.");
            }
        }
    }


    public class LoginIntentosValidacion
    {
        public static void ValidarBloqueo(
            loginDto loginDto,
            ConcurrentDictionary<string, DateTime> blockedUsers,
            ConcurrentDictionary<string, int> failedAttempts)
        {


            if (blockedUsers.ContainsKey(loginDto.identificador))
            {
                var tiempoBloqueo
                    = blockedUsers[loginDto.identificador];

                if (DateTime.UtcNow < tiempoBloqueo)
                {
                    throw new UnauthorizedException(
                        "La cuenta está bloqueada por 2 horas."
                    );
                }



                blockedUsers.TryRemove(
                    loginDto.identificador,
                    out _);

                failedAttempts.TryRemove(
                    loginDto.identificador,
                    out _);
            }
        }

        public static void ProcesarIntentoFallido(
            loginDto loginDto,
            ConcurrentDictionary<string, DateTime> blockedUsers,
            ConcurrentDictionary<string, int> failedAttempts)
        {
            if (failedAttempts.ContainsKey(loginDto.identificador))
            {
                failedAttempts[loginDto.identificador]++;
            }
            else
            {
                failedAttempts[loginDto.identificador] = 1;
            }

            if (failedAttempts[loginDto.identificador] >= 3)
            {
                blockedUsers[loginDto.identificador]
                    = DateTime.UtcNow.AddHours(2);

                failedAttempts.TryRemove(
                    loginDto.identificador,
                    out _);

                throw new UnauthorizedException(
                    "Cuenta bloqueada por 2 horas por exceder el máximo de intentos."
                );
            }
        }
    }
}

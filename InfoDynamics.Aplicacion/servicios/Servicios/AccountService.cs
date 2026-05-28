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


        public AccountService(
            IAuthTokenProcessor tokenProcessor,
            Iusuarioservicio usuarioService,
            IUserRepository userRepository)
        {
            _tokenProcessor = tokenProcessor;
            _usuarioService = usuarioService;
            _userRepository = userRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(
     loginDto loginDto)
        {
            LoginValidacion.Validar(loginDto);

            Usuario? user;


            if (int.TryParse(loginDto.identificador, out int numeroUsuario))
            {
                user = await _usuarioService.FindByIdAsync(
                    numeroUsuario);
            }
            else
            {
                user = await _usuarioService.FindByEmailAsync(
                    loginDto.identificador);
            }



            if (user != null)
            {
                await LoginIntentosValidacion.ValidarBloqueo(
                    user,
                    _usuarioService);
            }

            var usuarioValido = await _usuarioService.VerifyUser(
                loginDto.identificador,
                loginDto.contrasena);


            if (usuarioValido == null)
            {
                if (user != null)
                {
                    await LoginIntentosValidacion
                        .ProcesarIntentoFallido(
                            user,
                            _usuarioService);
                }

                throw new UnauthorizedException(
                    "Contraseña/Usuario incorrecta.");
            }


            usuarioValido.intentos = 0;
            usuarioValido.hora_bloqueo = null;
            usuarioValido.estado_cuenta = true;

            await _usuarioService.UpdateWithConcurrencyAsync(
                usuarioValido,
                usuarioValido.RowVersion);

            var rowVersionOriginal = usuarioValido.RowVersion;

            var (jwtToken, expirationDateInUtc)
                = _tokenProcessor.GenerateJwtToken(
                    usuarioValido);

            var refreshToken
                = _tokenProcessor.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc
                = DateTime.UtcNow.AddDays(7);

            usuarioValido.RefreshToken = refreshToken;

            usuarioValido.RefreshTokenExpiryTime
                = refreshTokenExpirationDateInUtc;

            await _usuarioService.UpdateWithConcurrencyAsync(
                usuarioValido,
                rowVersionOriginal);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "ACCESS_TOKEN",
                jwtToken,
                expirationDateInUtc);

            _tokenProcessor.WriteAuthTokenAsHttpOnlyCookie(
                "REFRESH_TOKEN",
                refreshToken,
                refreshTokenExpirationDateInUtc);

            return new LoginResponseDto
            {
                NoUsuario = usuarioValido.no_usuario,
                Token = jwtToken,
                EsManager = usuarioValido.es_manager
            };
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

        // CAMBIO:

        public async Task DesbloquearCuentaAsync(
            int noUsuario)
        {
            var user = await _usuarioService.FindByIdAsync(
                noUsuario);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "Usuario no encontrado.");
            }

            // Desbloqueo manual independiente del tiempoo
            //jj
            user.estado_cuenta = true;
            user.hora_bloqueo = null;
            user.intentos = 0;

            await _usuarioService.UpdateWithConcurrencyAsync(
                user,
                user.RowVersion);
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

            if (!loginDto.identificador.Contains("@") &&
                (!loginDto.identificador.All(char.IsDigit) ||
                 loginDto.identificador.Length != 7))
            {
                throw new BadRequestException(
                    "El número de empleado debe ser numérico.");
            }

            if (!loginDto.identificador.Contains("@") &&
                (!loginDto.identificador.All(char.IsDigit) ||
                 loginDto.identificador.Length != 7))
            {
                throw new BadRequestException(
                    "El número de empleado debe tener 7 dígitos.");
            }
        }
    }

    // CAMBIO:


    public class LoginIntentosValidacion
    {
        // Bloqueo temporal de 2 horas

        public static async Task ValidarBloqueo(
            Usuario user,
            Iusuarioservicio usuarioService)
        {
            // Bloqueo temporal de 2 horas
            if (
                user.estado_cuenta == false
                &&
                user.hora_bloqueo != null
            )
            {
                var finBloqueo
                    = user.hora_bloqueo.Value.AddHours(2);

                // Ya expiró el bloqueo


                if (DateTime.UtcNow >= finBloqueo)
                {
                    user.estado_cuenta = true;
                    user.intentos = 0;
                    user.hora_bloqueo = null;

                    await usuarioService
                        .UpdateWithConcurrencyAsync(
                            user,
                            user.RowVersion);
                }
                else
                {
                    throw new UnauthorizedException(
                        "La cuenta está bloqueada por 2 horas.");
                }
            }


            if (user.estado_cuenta != true)
            {
                throw new UnauthorizedException(
                    "La cuenta no está activa.");
            }
        }

        public static async Task ProcesarIntentoFallido(
            Usuario user,
            Iusuarioservicio usuarioService)
        {

            user.intentos++;

            if (user.intentos >= 3)
            {
                user.estado_cuenta = false;
                user.hora_bloqueo = DateTime.UtcNow;

                await usuarioService
                    .UpdateWithConcurrencyAsync(
                        user,
                        user.RowVersion);

                throw new UnauthorizedException(
                    "Cuenta bloqueada por 2 horas por exceder el máximo de intentos.");
            }

            await usuarioService
                .UpdateWithConcurrencyAsync(
                    user,
                    user.RowVersion);
        }
    }
}
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class ContrasenaService : IContrasenaService
    {
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly IGenericRepository<HistorialContrasena> _historialRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ContrasenaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _usuarioRepo = _unitOfWork.Repository<Usuario>();

            _historialRepo = _unitOfWork.Repository<HistorialContrasena>();
        }

        // ============================ CAMBIAR CONTRASEÑA ============================

        public async Task CambiarContrasenaAsync(
            int noUsuario,
            string contrasenaActual,
            string nuevaContrasena,
            string confirmarNuevaContrasena)
        {
            if (nuevaContrasena != confirmarNuevaContrasena)
            {
                throw new BadRequestException(
                    "Las contraseñas no coinciden.");
            }

            var usuario = await _usuarioRepo.GetAsync(
                u => u.no_usuario == noUsuario,
                tracked: true,
                includeProperties: "Contrasenas");

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    "Usuario no encontrado.");
            }

            if (!usuario.estado_cuenta)
            {
                throw new BadRequestException(
                    "La cuenta está inactiva.");
            }

            bool contrasenaCorrecta = BCrypt.Net.BCrypt.Verify(
                contrasenaActual,
                usuario.contrasena_hash);

            if (!contrasenaCorrecta)
            {
                throw new UnauthorizedException(
                    "La contraseña actual es incorrecta.");
            }

            ValidarSeguridadContrasena(
                nuevaContrasena,
                usuario.nombre);

            ValidarHistorialContrasenas(
                usuario,
                nuevaContrasena);

            string nuevoHash = BCrypt.Net.BCrypt.HashPassword(
                nuevaContrasena);

            usuario.contrasena_hash = nuevoHash;

            usuario.debe_cambiar_pass = false;

            usuario.intentos = 0;

            usuario.hora_bloqueo = null;

            var historial = new HistorialContrasena
            {
                no_usuario = usuario.no_usuario,

                contrasena_hash = nuevoHash,

                fecha_registro = DateTime.UtcNow,

            };

            await _historialRepo.AddAsync(historial);

            await _usuarioRepo.UpdateAsync(usuario);

            await _unitOfWork.SaveChangesAsync();
        }

        // ============================ RESTABLECER CONTRASEÑA ============================

        public async Task RestablecerContrasenaAsync(
            int noUsuario,
            string nuevaContrasena,
            string confirmarNuevaContrasena)
        {
            if (nuevaContrasena != confirmarNuevaContrasena)
            {
                throw new BadRequestException(
                    "Las contraseñas no coinciden.");
            }

            var usuario = await _usuarioRepo.GetAsync(
                u => u.no_usuario == noUsuario,
                tracked: true,
                includeProperties: "Contrasenas");

            if (usuario == null)
            {
                throw new EntityNotFoundException(
                    "Usuario no encontrado.");
            }

            ValidarSeguridadContrasena(
                nuevaContrasena,
                usuario.nombre);

            ValidarHistorialContrasenas(
                usuario,
                nuevaContrasena);

            string nuevoHash = BCrypt.Net.BCrypt.HashPassword(
                nuevaContrasena);

            usuario.contrasena_hash = nuevoHash;

            usuario.debe_cambiar_pass = true;

            usuario.estado_cuenta = true;

            usuario.intentos = 0;

            usuario.hora_bloqueo = null;

            var historial = new HistorialContrasena
            {
                no_usuario = usuario.no_usuario,

                contrasena_hash = nuevoHash,

                fecha_registro = DateTime.UtcNow,

            };

            await _historialRepo.AddAsync(historial);

            await _usuarioRepo.UpdateAsync(usuario);

            await _unitOfWork.SaveChangesAsync();
        }

        // ============================ VALIDAR SEGURIDAD ============================

        private void ValidarSeguridadContrasena(
            string contrasena,
            string nombreUsuario)
        {
            if (contrasena.Length < 12)
            {
                throw new BadRequestException(
                    "La contraseña debe tener mínimo 12 caracteres.");
            }

            bool mayuscula = contrasena.Any(char.IsUpper);

            bool minuscula = contrasena.Any(char.IsLower);

            if (!mayuscula || !minuscula)
            {
                throw new BadRequestException(
                    "La contraseña debe incluir mayúsculas y minúsculas.");
            }

            int cantidadNumeros = contrasena.Count(char.IsDigit);

            if (cantidadNumeros < 3)
            {
                throw new BadRequestException(
                    "La contraseña debe incluir al menos 3 números.");
            }

            int cantidadEspeciales = contrasena.Count(
                c => !char.IsLetterOrDigit(c));

            if (cantidadEspeciales < 3)
            {
                throw new BadRequestException(
                    "La contraseña debe incluir al menos 3 caracteres especiales.");
            }

            // NO REPETIR 3 CARACTERES CONSECUTIVOS

            for (int i = 0; i < contrasena.Length - 2; i++)
            {
                if (
                    contrasena[i] == contrasena[i + 1] &&
                    contrasena[i] == contrasena[i + 2]
                )
                {
                    throw new BadRequestException(
                        "La contraseña no puede contener caracteres repetidos consecutivos.");
                }
            }

            // NO SECUENCIAS

            for (int i = 0; i < contrasena.Length - 2; i++)
            {
                int a = contrasena[i];

                int b = contrasena[i + 1];

                int c = contrasena[i + 2];

                if (b == a + 1 && c == b + 1)
                {
                    throw new BadRequestException(
                        "La contraseña no puede contener secuencias consecutivas.");
                }
            }

            // NO CONTENER NOMBRE

            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                if (
                    contrasena.ToLower()
                    .Contains(nombreUsuario.ToLower())
                )
                {
                    throw new BadRequestException(
                        "La contraseña no puede contener el nombre del usuario.");
                }
            }
        }

        // ============================ VALIDAR HISTORIAL ============================

        private void ValidarHistorialContrasenas(
            Usuario usuario,
            string nuevaContrasena)
        {
            var ultimas3 = usuario.Contrasenas
                .OrderByDescending(c => c.fecha_registro)
                .Take(3)
                .ToList();

            foreach (var item in ultimas3)
            {
                bool reutilizada = BCrypt.Net.BCrypt.Verify(
                    nuevaContrasena,
                    item.contrasena_hash);

                if (reutilizada)
                {
                    throw new ConflictException(
                        "No puedes reutilizar las últimas 3 contraseñas.");
                }
            }
        }
    }
}

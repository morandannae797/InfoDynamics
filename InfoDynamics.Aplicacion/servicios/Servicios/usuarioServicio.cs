using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class UsuarioServicio : Iusuarioservicio
    {
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly IGenericRepository<Contrasena> _contrasenaRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UsuarioServicio(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _usuarioRepo = _unitOfWork.Repository<Usuario>();
            _contrasenaRepo = _unitOfWork.Repository<Contrasena>();
            _userRepository = userRepository;
        }

        public async Task<Usuario?> VerifyUser(string identificador, string contrasena)
        {
            Usuario? usuarioEncontrado;

            if (int.TryParse(identificador, out int numeroUsuario))
            {
                usuarioEncontrado = await _usuarioRepo.GetAsync(
                    u => u.no_usuario == numeroUsuario,
                    tracked: true,
                    includeProperties: "Contrasenas");
            }
            else
            {
                usuarioEncontrado = await _usuarioRepo.GetAsync(
                    u => u.email.ToLower() == identificador.ToLower(),
                    tracked: true,
                    includeProperties: "Contrasenas");
            }

            if (usuarioEncontrado == null)
                return null;

            var contrasenaActiva = usuarioEncontrado.Contrasenas
                .FirstOrDefault(c => c.estado == "Activa");

            if (contrasenaActiva == null)
                return null;

            bool esValida = BCrypt.Net.BCrypt.Verify(
                contrasena,
                contrasenaActiva.contrasena);

            return esValida ? usuarioEncontrado : null;
        }

        public async Task<Usuario> CreateFromDtoAsync(UsuarioCreateDTO dto)
        {
            ValidarSeguridadContrasena(dto.Contrasena, dto.Nombre);

            //Historial de contraseñas------------------------------------------
            await ValidarHistorialContrasenas(
                dto.NoUsuario,
                dto.Contrasena
            );




            var existente = await _usuarioRepo.GetByIdAsync(dto.NoUsuario);

            if (existente != null)
                throw new ConflictException(
                    $"Ya existe un usuario con el número {dto.NoUsuario}."
                );

            var emailExistente = await _usuarioRepo.GetAsync(
                u => u.email == dto.Email);

            if (emailExistente != null)
                throw new ConflictException(
                    $"El correo {dto.Email} ya está registrado."
                );

            var usuario = new Usuario
            {
                no_usuario = dto.NoUsuario,
                nombre = dto.Nombre,
                ap_paterno = dto.ApPaterno,
                ap_materno = dto.ApMaterno,
                email = dto.Email,
                rol = dto.Rol,
                estado_cuenta = "Activa"
            };

            await _usuarioRepo.AddAsync(usuario);

            var contrasena = new Contrasena
            {
                contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                fecha_creacion = DateTime.UtcNow,
                estado = "Activa",
                es_temporal = false,
                no_usuario = dto.NoUsuario
            };

            await _contrasenaRepo.AddAsync(contrasena);

            await _unitOfWork.SaveChangesAsync();

            return usuario;
        }







       //---------------------------------------------------S2.6--------------------------------------------
        private void ValidarSeguridadContrasena(string contrasena, string nombreUsuario)
        {
            // -------------------------------------------S2.6.1.1----------------------------------------
            if (contrasena.Length < 12)
            {
                throw new BadRequestException(
                    "La contraseña debe tener mínimo 12 caracteres."
                );
            }

            // -------------------------------------------S2.6.1.2-----------------------------------------
            bool Mayuscula = contrasena.Any(char.IsUpper);
            bool Minuscula = contrasena.Any(char.IsLower);

            if (!Mayuscula || !Minuscula)
            {
                throw new BadRequestException(
                    "La contraseña debe incluir mayúsculas y minúsculas."
                );
            }

            // ===========================================S2.6.1.3============================================
            int cantidadNumeros = contrasena.Count(char.IsDigit);

            if (cantidadNumeros < 3)
            {
                throw new BadRequestException(
                    "La contraseña debe incluir al menos 3 números."
                );
            }

            // -----------------------------------------S2.6.1.4---------------------------------------
            int cantidadEspeciales = contrasena.Count(
                c => !char.IsLetterOrDigit(c));

            if (cantidadEspeciales < 3)
            {
                throw new BadRequestException(
                    "La contraseña debe incluir al menos 3 caracteres especiales."
                );
            }

            // -----------------------------------------S2.6.1.5-----------------------------------------
            for (int i = 0; i < contrasena.Length - 2; i++)
            {
                if (contrasena[i] == contrasena[i + 1] &&
                    contrasena[i] == contrasena[i + 2])
                {
                    throw new BadRequestException(
                        "La contraseña no puede contener caracteres repetidos consecutivos."
                    );
                }
            }


            // -----------------------------------------S2.6.1.7-----------------------------------------
            // No permitir incluir el nombre del usuario en la contraseña
            if (!string.IsNullOrWhiteSpace(nombreUsuario))
            {
                string nombreLower = nombreUsuario.ToLower();
                string contrasenaLower = contrasena.ToLower();

                if (contrasenaLower.Contains(nombreLower))
                {
                    throw new BadRequestException(
                        "La contraseña no puede contener el nombre del usuario."
                    );
                }
            }

        }






        //-------------------------------------------S2.6.1.6------------------------------------------------
        //Metodo para validar el historial de contraseñas.
        private async Task ValidarHistorialContrasenas(
    int noUsuario,
    string nuevaContrasena)
        {
            var usuario = await _usuarioRepo.GetAsync(
                u => u.no_usuario == noUsuario,
                tracked: false,
                includeProperties: "Contrasenas"
            );

            if (usuario == null)
                return;

            var ultimas3 = usuario.Contrasenas
                .OrderByDescending(c => c.fecha_creacion)
                .Take(3)
                .ToList();

            foreach (var c in ultimas3)
            {
                if (BCrypt.Net.BCrypt.Verify(nuevaContrasena, c.contrasena))
                {
                    throw new BadRequestException(
                        "No puedes reutilizar las últimas 3 contraseñas."
                    );
                }
            }
        }


        //----------------------------------------------------------------------------------------------------



        public async Task<Usuario?> FindByEmailAsync(string email)
        {
            return await _usuarioRepo.GetAsync(
                u => u.email == email,
                tracked: true,
                includeProperties: "Contrasenas");
        }

        public async Task<Usuario?> FindByIdAsync(int id)
        {
            return await _usuarioRepo.GetAsync(
                u => u.no_usuario == id,
                tracked: true,
                includeProperties: "Contrasenas");
        }

        public Task<bool> IsInRoleAsync(Usuario user, string role)
        {
            return Task.FromResult(user.rol == role);
        }

        public async Task<Usuario> UpdateWithConcurrencyAsync(
            Usuario usuarioActualizado,
            byte[] rowVersion)
        {
            var usuarioExistente = await _usuarioRepo.GetByIdAsync(
                usuarioActualizado.no_usuario);

            if (usuarioExistente == null)
                throw new KeyNotFoundException(
                    "Usuario no encontrado."
                );

            usuarioExistente.email = usuarioActualizado.email;
            usuarioExistente.nombre = usuarioActualizado.nombre;
            usuarioExistente.ap_paterno = usuarioActualizado.ap_paterno;
            usuarioExistente.ap_materno = usuarioActualizado.ap_materno;
            usuarioExistente.rol = usuarioActualizado.rol;
            usuarioExistente.estado_cuenta = usuarioActualizado.estado_cuenta;

            usuarioExistente.RefreshToken = usuarioActualizado.RefreshToken;
            usuarioExistente.RefreshTokenExpiryTime =
                usuarioActualizado.RefreshTokenExpiryTime;

            _usuarioRepo.SetOriginalConcurrencyToken(
                usuarioExistente,
                rowVersion);

            try
            {
                await _usuarioRepo.UpdateAsync(usuarioExistente);

                await _unitOfWork.SaveChangesAsync();

                return usuarioExistente;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(
                    "El usuario fue modificado por otro proceso. Recarga los datos y reintenta.",
                    ex);
            }
        }
    }
}
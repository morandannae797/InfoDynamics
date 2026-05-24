using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class UsuarioServicio : Iusuarioservicio
    {
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly IGenericRepository<HistorialContrasena> _contrasenaRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UsuarioServicio(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _usuarioRepo = _unitOfWork.Repository<Usuario>();
            _contrasenaRepo = _unitOfWork.Repository<HistorialContrasena>();
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

            if (!usuarioEncontrado.estado_cuenta)
                return null;

            if (usuarioEncontrado == null)
                return null;

            if (!usuarioEncontrado.estado_cuenta)
                return null;

            bool esValida;

            try
            {
                esValida = BCrypt.Net.BCrypt.Verify(
                    contrasena,
                    usuarioEncontrado.contrasena_hash
                );
            }
            catch
            {
                return null;
            }

            return esValida ? usuarioEncontrado : null;
        }

        public async Task<Usuario> CreateFromDtoAsync(UsuarioCreateDTO dto)
        {
            var existente = await _usuarioRepo.GetByIdAsync(dto.NoUsuario);

            if (existente != null)
                throw new ConflictException($"Ya existe un usuario con el número {dto.NoUsuario}.");

            var emailExistente = await _usuarioRepo.GetAsync(u => u.email == dto.Email);

            if (emailExistente != null)
                throw new ConflictException($"El correo {dto.Email} ya está registrado.");

            var usuario = new Usuario
            {
                no_usuario = dto.NoUsuario,
                nombre = dto.Nombre,
                ap_paterno = dto.ApPaterno,
                ap_materno = dto.ApMaterno,
                email = dto.Email,
                es_manager = dto.EsManager,
                estado_cuenta = true,
                contrasena_hash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                debe_cambiar_pass = true,
                intentos = 0,
                hora_bloqueo = null,
                RefreshToken = null,
                RefreshTokenExpiryTime = null
            };

            await _usuarioRepo.AddAsync(usuario);

            var contrasenaHistorial = new HistorialContrasena
            {
                contrasena_hash = usuario.contrasena_hash,
                fecha_registro = DateTime.UtcNow,
                es_temporal = false,
                no_usuario = dto.NoUsuario
            };

            await _contrasenaRepo.AddAsync(contrasenaHistorial);

            await _unitOfWork.SaveChangesAsync();

            return usuario;
        }

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
            return Task.FromResult(
                role == "Manager"
                    ? user.es_manager
                    : !user.es_manager
            );
        }

        public async Task<Usuario> UpdateWithConcurrencyAsync(Usuario usuarioActualizado, byte[] rowVersion)
        {
            var usuarioExistente = await _usuarioRepo.GetByIdAsync(usuarioActualizado.no_usuario);

            if (usuarioExistente == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            usuarioExistente.email = usuarioActualizado.email;
            usuarioExistente.nombre = usuarioActualizado.nombre;
            usuarioExistente.ap_paterno = usuarioActualizado.ap_paterno;
            usuarioExistente.ap_materno = usuarioActualizado.ap_materno;
            usuarioExistente.es_manager = usuarioActualizado.es_manager;
            usuarioExistente.estado_cuenta = usuarioActualizado.estado_cuenta;
            usuarioExistente.debe_cambiar_pass = usuarioActualizado.debe_cambiar_pass;

            // No actualizo contrasena_hash aquí porque el cambio de contraseña
            // debe hacerse en CambiarContrasenaDto o en otro metodo especifico.
            // Asi evitas modificar contraseñas por accidente desde Update.

            // No actualizo intentos ni hora_bloqueo aquí porque son campos de seguridad/login.
            // Se deberian modificar desde la logica de autenticacion.

            // No actualizo RefreshToken aquí porque se maneja desde login/refresh token.
            // Se mantiene la logica original de seguridad separada.

            _usuarioRepo.SetOriginalConcurrencyToken(usuarioExistente, rowVersion);

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

        // AQUI ACOMODAs plis la verdad prefiero que lo hagas tu para que te familiarices con tu codigo de
        // servicio, pero basicamente es un servicio de validacion que se encarga de validar los datos de entrada
        // Ahi le agregas lo que falte de validaciones (SI ES QUE FALTAN),
        // Las que deben de estar son como por ejemplo validar que el numero de empleado sea de 7 digitos,
        // validar que el correo sea unico, validar que el numero de empleado sea unico, etc.


        public class UsuarioValidacionService
        {
            // Validar campos obligatorios.
            // Validar número de empleado de 7 digitos.
            // Validar correo único.
            // Validar número de empleado único.
        }
    }
}
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

            bool esValida = BCrypt.Net.BCrypt.Verify(contrasena, contrasenaActiva.contrasena);

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
            await _usuarioRepo.AddAsync(usuario);
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
            return Task.FromResult(user.rol == role);
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
            usuarioExistente.rol = usuarioActualizado.rol;
            usuarioExistente.estado_cuenta = usuarioActualizado.estado_cuenta;

            usuarioExistente.RefreshToken = usuarioActualizado.RefreshToken;
            usuarioExistente.RefreshTokenExpiryTime = usuarioActualizado.RefreshTokenExpiryTime;

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
    }
}
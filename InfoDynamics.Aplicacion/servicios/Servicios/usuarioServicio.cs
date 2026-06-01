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
        private readonly IGenericRepository<Usuario_manager> _usuarioManagerRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UsuarioServicio(IUnitOfWork unitOfWork, IUserRepository userRepository, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _usuarioRepo = _unitOfWork.Repository<Usuario>();
            _contrasenaRepo = _unitOfWork.Repository<HistorialContrasena>();
            _usuarioManagerRepo = _unitOfWork.Repository<Usuario_manager>();
            _userRepository = userRepository;
            _emailService = emailService;
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

            var ContraTemporal = GeneradorContrasena.Generarcontrasenatemporal();
            var usuario = new Usuario
            {
                no_usuario = dto.NoUsuario,
                nombre = dto.Nombre,
                ap_paterno = dto.ApPaterno,
                ap_materno = dto.ApMaterno,
                email = dto.Email,
                es_manager = dto.EsManager,
                estado_cuenta = true,
                contrasena_hash = BCrypt.Net.BCrypt.HashPassword(ContraTemporal),
                debe_cambiar_pass = true,
                intentos = 0,
                hora_bloqueo = null,
                RefreshToken = null,
                RefreshTokenExpiryTime = null
            };

            await _usuarioRepo.AddAsync(usuario);

            await _emailService.SendTemporaryPasswordAsync(
            usuario.email,
            usuario.nombre,
            ContraTemporal,
            "https://www.infodynamics.lat/HTML/Login.html");

            var contrasenaHistorial = new HistorialContrasena
            {
                contrasena_hash = usuario.contrasena_hash,
                fecha_registro = DateTime.UtcNow,

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

        public async Task<object?> ObtenerEmpleadoDeManager(int managerId, int empleadoId)
        {
            var repoRelacion = _unitOfWork.Repository<Usuario_manager>();

            var relacion = await repoRelacion.GetAsync(
                x => x.no_usuario_manager == managerId
                  && x.no_usuario == empleadoId);

            if (relacion == null)
                return null;

            var repoUsuario = _unitOfWork.Repository<Usuario>();

            var usuario = await repoUsuario.GetAsync(
                x => x.no_usuario == empleadoId);

            if (usuario == null)
                return null;

            return new
            {
                usuario.no_usuario,
                nombre_completo = string.Join(" ",
        new[]
        {
            usuario.nombre,
            usuario.ap_paterno,
            usuario.ap_materno
        }.Where(x => !string.IsNullOrWhiteSpace(x))),
                usuario.email
            };
        }

        public async Task<object?> ObtenerPerfilSimpleAsync(int id)
        {
            var repoUsuario = _unitOfWork.Repository<Usuario>();

            var usuario = await repoUsuario.GetAsync(
                x => x.no_usuario == id);

            if (usuario == null)
                return null;

            return new
            {
                usuario.no_usuario,
                nombre_completo = string.Join(" ",
                    new[]
                    {
                usuario.nombre,
                usuario.ap_paterno,
                usuario.ap_materno
                    }.Where(x => !string.IsNullOrWhiteSpace(x))),
                usuario.email
            };
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


        public class UsuarioValidacionService
        {
            private readonly IGenericRepository<Usuario> _usuarioRepo;

            public UsuarioValidacionService(IUnitOfWork unitOfWork)
            {
                _usuarioRepo = unitOfWork.Repository<Usuario>();
            }



            public void ValidarNumeroEmpleado(int noUsuario)
            {
                if (noUsuario.ToString().Length > 7)
                    throw new BadRequestException("El nummero de empleado no puede tener mas de 7 digitos.");
            }

            public async Task ValidarNoUsuarioUnicoAsync(int noUsuario)
            {
                var existente = await _usuarioRepo.GetByIdAsync(noUsuario);

                if (existente != null)
                    throw new ConflictException($"Ya existe un usuario con el numero {noUsuario}.");
            }

            public async Task ValidarEmailUnicoAsync(string email, int? excluirNoUsuario = null)
            {
                var existente = await _usuarioRepo.GetAsync(
                    u => u.email == email
                    && (excluirNoUsuario == null || u.no_usuario != excluirNoUsuario)
                );

                if (existente != null)
                    throw new ConflictException($"El correo {email} ya esta registrado.");
            }
        }
    }
}
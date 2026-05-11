using BCrypt.Net;
using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Aplicacion.servicios.Servicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class UsuarioServicio : Iusuarioservicio
    {
        private readonly IGenericRepository<Usuario> _usuarioRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UsuarioServicio(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _usuarioRepo = _unitOfWork.Repository<Usuario>();
            _userRepository = userRepository;
        }
        public async Task<Usuario?> VerifyUser(string identificador, string contrasena)
        {
            var usuarioEncontrado = await _unitOfWork.Repository<Usuario>().GetAsync(u => u.email.ToLower() == identificador.ToLower() || u.no_usuario.ToString() == identificador.ToString());


            if (usuarioEncontrado == null) return null;
             
            bool esValida = BCrypt.Net.BCrypt.Verify(contrasena, usuarioEncontrado.contrasena);

            return esValida ? usuarioEncontrado : null;
        }
        public async Task<Usuario> CreateFromDtoAsync(UsuarioCreateDTO dto)
        {
            var existente = await _usuarioRepo.GetByIdAsync(dto.no_usuario);
            if (existente != null)
                throw new ConflictException($"Ya existe un usuario con el número {dto.no_usuario}.");

            var emailExistente = await _usuarioRepo.GetAsync(u => u.email == dto.email);
            if (emailExistente != null)
                throw new ConflictException($"El correo {dto.email} ya está registrado.");

            var usuario = new Usuario
            {
                no_usuario = dto.no_usuario,
                nombre = dto.nombre,
                ap_paterno = dto.ap_paterno,
                ap_maternos = dto.ap_maternos,
                email = dto.email,
                rol = dto.rol,
                contrasena = BCrypt.Net.BCrypt.HashPassword(dto.contrasena),
                RowVersion = Guid.NewGuid().ToByteArray()
            };

            await _usuarioRepo.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();
            return usuario;
        }
        public async Task<Usuario?> FindByEmailAsync(string email)
        {
            return await _usuarioRepo.GetAsync(u => u.email == email);
        }
        public async Task<Usuario?> FindByIdAsync(int id)
        {
            return await _usuarioRepo.GetByIdAsync(id);
        }
        public Task<bool> IsInRoleAsync(Usuario user, string role)
        => Task.FromResult(user.rol == role);

    
        public async Task<Usuario> UpdateWithConcurrencyAsync(Usuario usuarioActualizado, byte[] rowVersion)
        {
            
            var usuarioExistente = await _usuarioRepo.GetByIdAsync(usuarioActualizado.no_usuario);
            if (usuarioExistente == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            usuarioExistente.email = usuarioActualizado.email;
            usuarioExistente.nombre = usuarioActualizado.nombre;
            usuarioExistente.ap_paterno = usuarioActualizado.ap_paterno;
            usuarioExistente.ap_maternos = usuarioActualizado.ap_maternos;
            usuarioExistente.rol = usuarioActualizado.rol;

            if (!string.IsNullOrEmpty(usuarioActualizado.contrasena))
                usuarioExistente.contrasena = BCrypt.Net.BCrypt.HashPassword(usuarioActualizado.contrasena);


            _usuarioRepo.SetOriginalConcurrencyToken(usuarioExistente, rowVersion);
            await _usuarioRepo.UpdateAsync(usuarioExistente);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return usuarioExistente;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Alguien más modificó el registro
                throw new InvalidOperationException("El usuario fue modificado por otro proceso. Recarga los datos y reintenta.", ex);
            }
        }
    }
}

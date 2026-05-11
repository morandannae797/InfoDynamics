using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping
{
    public interface Iusuarioservicio
    {
        Task<Usuario> CreateFromDtoAsync(UsuarioCreateDTO dto);
        Task<Usuario?> FindByEmailAsync(string email);
        Task<Usuario?> FindByIdAsync(int id);
        Task<Usuario> UpdateWithConcurrencyAsync(Usuario usuarioActualizado, byte[] rowVersion);
        Task<bool> IsInRoleAsync(Usuario user, string role);
        Task<Usuario?> VerifyUser(string identificador, string contrasena);
    }
}

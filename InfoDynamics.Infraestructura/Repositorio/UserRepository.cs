using System;
using System.Collections.Generic;
using System.Text;
using InfoDynamics.Infraestructura.Contexto;
using InfoDynamics.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using InfoDynamics.Aplicacion.Abstracts;

namespace InfoDynamics.Infraestructura.Repositorio
{
    public class UserRepository : IUserRepository
    {
        private readonly EmployeesDbContext _context;
    

        public UserRepository(EmployeesDbContext context)
        {
            _context = context; }
    public async Task<Usuario?> GetUserbyRefreshToken(string refreshToken)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            return user;
        }
        public async Task SaveRefreshTokenAsync(int noUsuario, string refreshToken, DateTime expires)
        {
            var usuario = await _context.Usuarios.FindAsync(noUsuario);
            if (usuario == null) return;

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = expires;

            await _context.SaveChangesAsync();
        }
    } 

}

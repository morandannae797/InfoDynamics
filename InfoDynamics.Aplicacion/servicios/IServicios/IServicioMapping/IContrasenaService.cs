using InfoDynamics.Aplicacion.dtos;

namespace InfoDynamics.Aplicacion.servicio.IServicios
{
    public interface IContrasenaService
    {
        Task CambiarContrasenaAsync(
            int noUsuario,
            string contrasenaActual,
            string nuevaContrasena,
            string confirmarNuevaContrasena);

        Task RestablecerContrasenaAsync(
            int noUsuario,
            string nuevaContrasena,
            string confirmarNuevaContrasena);
    }
}
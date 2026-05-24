using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;


namespace InfoDynamics.Aplicacion.Servicios.Servicios
{

        public class VacacionValidacionService
        {
            // Valida solicitud y periodo solicitado.
        }

        public class VacacionSolicitudService
        {
            // Registra solicitud de vacaciones con estado pendiente.
        }

        public class VacacionDecisionService
        {
            // Aprueba o rechaza vacaciones.
            // Si aprueba, registra horas automáticamente.
        }

        public class VacacionNotificacionService
        {
            // Notifica al administrador y al empleado por correo.
        }
    
}

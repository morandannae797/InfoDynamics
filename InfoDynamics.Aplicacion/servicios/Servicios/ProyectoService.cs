using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class RegistroProyectoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegistroProyectoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CrearProyectoAsync(int idEmpresa, bool esCobrable)
        {
            string prefijo = esCobrable ? "L" : "M";

            var proyectos = await _unitOfWork
                .Repository<Proyecto>()
                .GetAllAsync();

            var ultimoProyecto = proyectos
                .Where(p => p.codigo.StartsWith(prefijo))
                .OrderByDescending(p => p.codigo)
                .FirstOrDefault();

            int siguienteNumero = 1;

            if (ultimoProyecto != null)
            {
                string numeroTexto = ultimoProyecto.codigo.Substring(1);

                siguienteNumero = int.Parse(numeroTexto) + 1;
            }

            string codigoGenerado = $"{prefijo}{siguienteNumero.ToString("D6")}";

            var nuevoProyecto = new Proyecto
            {
                codigo = codigoGenerado,
                es_cobrable = esCobrable,
                id_empresa = idEmpresa
            };

            await _unitOfWork
                .Repository<Proyecto>()
                .AddAsync(nuevoProyecto);
        }

        public async Task CrearProyectosEmpresaAsync(int idEmpresa, string tipoProyecto)
        {
            if (tipoProyecto == "Cobrable")
            {
                await CrearProyectoAsync(idEmpresa, true);
            }
            else if (tipoProyecto == "NoCobrable")
            {
                await CrearProyectoAsync(idEmpresa, false);
            }
            else if (tipoProyecto == "Ambos")
            {
                await CrearProyectoAsync(idEmpresa, true);
                await CrearProyectoAsync(idEmpresa, false);
            }
            else
            {
                throw new BadRequestException("El tipo de proyecto no es válido.");
            }
        }
    }
}
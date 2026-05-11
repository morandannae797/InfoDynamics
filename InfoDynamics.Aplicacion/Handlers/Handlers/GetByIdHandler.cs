using InfoDynamics.Aplicacion.Handlers.Comandos;
using InfoDynamics.Aplicacion.servicio.IServicios;
using MediatR;

namespace InfoDynamics.Aplicacion.Handlers.Handlers
{
	public class GetByIdHandler<T> : IRequestHandler<GetByIdCommand<T>> where T : class
	{
		public GetByIdHandler()
		{

		}
		public Task Handle(GetByIdCommand<T> request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
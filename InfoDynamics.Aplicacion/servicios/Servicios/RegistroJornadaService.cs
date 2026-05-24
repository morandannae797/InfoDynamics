using AutoMapper;
using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using System;
using System.Threading.Tasks;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class RegistroJornadaService
        : WriteServiceAsync<Registro, RegistroCreateDto, RegistroUpdateDto>
    {

        public RegistroJornadaService(
            IUnitOfWork unitOfWork,
            IMapper mapper
        ) : base(unitOfWork, mapper)
        {
        }

        public override async Task AddAsync(RegistroCreateDto dto)
        {

            if (dto.Horas < 0)
            {
                throw new BadRequestException(
                    "Las horas no pueden ser negativas."
                );
            }



            var proyecto = await _unitOfWork
                .Repository<Proyecto>()
                .GetAsync(p => p.codigo == dto.Codigo);



            if (proyecto == null)
            {
                throw new EntityNotFoundException(
                    "El proyecto no existe."
                );
            }


            if (proyecto.id_empresa <= 0)
            {
                throw new BadRequestException(
                    "El proyecto no tiene empresa asignada."
                );
            }


            if (
                !proyecto.codigo.StartsWith(
                    "L",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                !proyecto.codigo.StartsWith(
                    "M",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "El código del proyecto no tiene clasificación válida."
                );
            }


            await base.AddAsync(dto);
        }





        public override async Task UpdateAsync(RegistroUpdateDto dto)
        {


            if (dto.Horas < 0)
            {
                throw new BadRequestException(
                    "Las horas no pueden ser negativas."
                );
            }


            var proyecto = await _unitOfWork
                .Repository<Proyecto>()
                .GetAsync(p => p.codigo == dto.Codigo);

            if (proyecto == null)
            {
                throw new EntityNotFoundException(
                    "El proyecto no existe."
                );
            }



            if (proyecto.id_empresa <= 0)
            {
                throw new BadRequestException(
                    "El proyecto no tiene empresa asignada."
                );
            }


            if (
                !proyecto.codigo.StartsWith(
                    "L",
                    StringComparison.OrdinalIgnoreCase
                )
                &&
                !proyecto.codigo.StartsWith(
                    "M",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new BadRequestException(
                    "El código del proyecto no tiene clasificación válida."
                );
            }


            await base.UpdateAsync(dto);
        }
    }
}
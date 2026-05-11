using AutoMapper;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Dominio.Entidades;
using static InfoDynamics.Aplicacion.dtos.VacacionDto;

namespace InfoDynamics.Aplicacion.mapeo
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Empresa, EmpresaDto>()
    .ForMember(dest => dest.IDEmpresa, opt => opt.MapFrom(src => src.id_empresa));

            CreateMap<EmpresaDto, Empresa>()
                .ForMember(dest => dest.id_empresa, opt => opt.MapFrom(src => src.IDEmpresa));

            CreateMap<Periodo, PeriodoDto>()
                .ForMember(dest => dest.PeriodoID, opt => opt.MapFrom(src => src.id_periodo))
                .ReverseMap();
            CreateMap<PeriodoDto, Periodo>()
               .ForMember(dest => dest.id_periodo, opt => opt.MapFrom(src => src.PeriodoID))
               .ReverseMap();

            CreateMap<Registro, RegistroJornadaDto>()
                .ForMember(dest => dest.RegistroID, opt => opt.MapFrom(src => src.id_registro))
                .ReverseMap();
            CreateMap< RegistroJornadaDto, Registro>()
               .ForMember(dest => dest.id_registro, opt => opt.MapFrom(src => src.RegistroID))
               .ReverseMap();

            CreateMap<UsuarioCreateDTO, Usuario>()
    .ForMember(dest => dest.contrasena, opt => opt.Ignore())
    .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
    .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
    .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
    .ForMember(dest => dest.IdEmpresaNavigation, opt => opt.Ignore())
    .ForMember(dest => dest.RegistrosJornada, opt => opt.Ignore())
    .ForMember(dest => dest.VacacionesSolicitadas, opt => opt.Ignore())
    .ForMember(dest => dest.VacacionesAprobadas, opt => opt.Ignore());

            CreateMap<Usuario, UsuarioResponseDTO>();



            // Command
            CreateMap<VacacionDto.VacacionCreateDTO, Vacacion>()
               
                 .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.SolicitanteId))
                
                 .ForMember(dest => dest.id_vacacion, opt => opt.Ignore())
                 .ForMember(dest => dest.RowVersion, opt => opt.Ignore());

           
            CreateMap<Vacacion, VacacionDto.VacacionResponseDTO>()
                .ForMember(dest => dest.VacacionId, opt => opt.MapFrom(src => src.id_vacacion))
                .ForMember(dest => dest.EstadoAprobacion, opt => opt.MapFrom(src => src.estado))
               
                .ForMember(dest => dest.NombreSolicitante, opt => opt.MapFrom(src => src.Solicitante.nombre))
                .ForMember(dest => dest.NombreAprobador, opt => opt.MapFrom(src => src.Aprobador != null ? src.Aprobador.nombre : null));

        }
    }

}
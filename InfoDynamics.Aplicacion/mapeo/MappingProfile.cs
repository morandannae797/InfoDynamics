using AutoMapper;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Dominio.Entidades;

namespace InfoDynamics.Aplicacion.mapeo
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            CreateMap<Usuario_manager, UsuarioManagerResponseDto>()
                .ForMember(dest => dest.NoUsuario,
                    opt => opt.MapFrom(src => src.no_usuario))
                .ForMember(dest => dest.NoUsuarioManager,
                    opt => opt.MapFrom(src => src.no_usuario_manager));


            CreateMap<UsuarioManagerCreateDto, Usuario_manager>()
                .ForMember(dest => dest.no_usuario,
                    opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.no_usuario_manager,
                    opt => opt.MapFrom(src => src.NoUsuarioManager));


            CreateMap<UsuarioManagerUpdateDto, Usuario_manager>()
                .ForMember(dest => dest.no_usuario,
                    opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.no_usuario_manager,
                    opt => opt.MapFrom(src => src.NoUsuarioManager));



            CreateMap<Empresa, EmpresaDto>()
                .ForMember(dest => dest.IdEmpresa, opt => opt.MapFrom(src => src.id_empresa))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre));
                //.ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<EmpresaCreateDto, Empresa>()
                .ForMember(dest => dest.id_empresa, opt => opt.Ignore())
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                //.ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.Proyectos, opt => opt.Ignore());

            CreateMap<EmpresaDto, Empresa>()
       .ForMember(dest => dest.id_empresa, opt => opt.MapFrom(src => src.IdEmpresa))
       .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
       //.ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
       .ForMember(dest => dest.Proyectos, opt => opt.Ignore());



            CreateMap<Periodo, PeriodoResponseDto>()
                .ForMember(dest => dest.PeriodoId, opt => opt.MapFrom(src => src.id_periodo))
                .ForMember(dest => dest.FechaInicio, opt => opt.MapFrom(src => ToDateTime(src.fecha_inicio)))
                .ForMember(dest => dest.FechaFin, opt => opt.MapFrom(src => ToDateTime(src.fecha_fin)))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.estado))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));

            CreateMap<PeriodoCreateDto, Periodo>()
                .ForMember(dest => dest.id_periodo, opt => opt.Ignore())
                .ForMember(dest => dest.fecha_inicio, opt => opt.MapFrom(src => ToDateOnly(src.FechaInicio)))
                .ForMember(dest => dest.fecha_fin, opt => opt.MapFrom(src => ToDateOnly(src.FechaFin)))
                .ForMember(dest => dest.estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.Registros, opt => opt.Ignore());
CreateMap<PeriodoUpdateDto, Periodo>()
    .ForMember(dest => dest.id_periodo, opt => opt.MapFrom(src => src.PeriodoId))
    .ForMember(dest => dest.fecha_inicio, opt => opt.MapFrom(src => ToDateOnly(src.FechaInicio)))
    .ForMember(dest => dest.fecha_fin, opt => opt.MapFrom(src => ToDateOnly(src.FechaFin)))
    .ForMember(dest => dest.estado, opt => opt.MapFrom(src => src.Estado))
    .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
    .ForMember(dest => dest.Registros, opt => opt.Ignore());


            // REGISTRO RESPONSE
            CreateMap<Registro, RegistroResponseDto>()
                .ForMember(dest => dest.RegistroId, opt => opt.MapFrom(src => src.id_registro))
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.fecha))
                .ForMember(dest => dest.Horas, opt => opt.MapFrom(src => src.horas))
                .ForMember(dest => dest.NoUsuario, opt => opt.MapFrom(src => src.no_usuario))
                .ForMember(dest => dest.PeriodoId, opt => opt.MapFrom(src => src.id_periodo))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.codigo));


            // REGISTRO CREATE
            CreateMap<RegistroCreateDto, Registro>()
                .ForMember(dest => dest.id_registro, opt => opt.Ignore())
                .ForMember(dest => dest.fecha, opt => opt.MapFrom(src => src.Fecha))
                .ForMember(dest => dest.horas, opt => opt.MapFrom(src => src.Horas))
                .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.id_periodo, opt => opt.MapFrom(src => src.PeriodoId))
                .ForMember(dest => dest.codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.no_usuarioNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.id_periodoNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.id_proyectoNavigation, opt => opt.Ignore());


            // REGISTRO UPDATE
            CreateMap<RegistroUpdateDto, Registro>()
                .ForMember(dest => dest.id_registro, opt => opt.MapFrom(src => src.RegistroId))
                .ForMember(dest => dest.fecha, opt => opt.MapFrom(src => src.Fecha))
                .ForMember(dest => dest.horas, opt => opt.MapFrom(src => src.Horas))
                .ForMember(dest => dest.id_periodo, opt => opt.MapFrom(src => src.PeriodoId))
                .ForMember(dest => dest.codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.no_usuario, opt => opt.Ignore())
                .ForMember(dest => dest.no_usuarioNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.id_periodoNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.id_proyectoNavigation, opt => opt.Ignore());

            // USUARIO CREATE
            CreateMap<UsuarioCreateDTO, Usuario>()
                .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.ap_paterno, opt => opt.MapFrom(src => src.ApPaterno))
                .ForMember(dest => dest.ap_materno, opt => opt.MapFrom(src => src.ApMaterno))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.es_manager, opt => opt.MapFrom(src => src.EsManager))
                .ForMember(dest => dest.estado_cuenta, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.contrasena_hash,
                    opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Contrasena)))
                .ForMember(dest => dest.debe_cambiar_pass, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.intentos, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.hora_bloqueo, opt => opt.Ignore())
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.contrasena_hash, opt => opt.Ignore())
                .ForMember(dest => dest.Registros, opt => opt.Ignore())
                .ForMember(dest => dest.Vacacionid_administradorNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.Vacacionno_usuarioNavigations, opt => opt.Ignore());


            // USUARIO UPDATE
            CreateMap<UsuarioUpdateDto, Usuario>()
                .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.ap_paterno, opt => opt.MapFrom(src => src.ApPaterno))
                .ForMember(dest => dest.ap_materno, opt => opt.MapFrom(src => src.ApMaterno))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.es_manager, opt => opt.MapFrom(src => src.EsManager))
                .ForMember(dest => dest.estado_cuenta, opt => opt.MapFrom(src => src.EstadoCuenta))
                .ForMember(dest => dest.debe_cambiar_pass, opt => opt.MapFrom(src => src.DebeCambiarPass))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.contrasena_hash, opt => opt.Ignore())
                .ForMember(dest => dest.intentos, opt => opt.Ignore())
                .ForMember(dest => dest.hora_bloqueo, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiryTime, opt => opt.Ignore())
                .ForMember(dest => dest.contrasena_hash, opt => opt.Ignore())
                .ForMember(dest => dest.Registros, opt => opt.Ignore())
                .ForMember(dest => dest.Vacacionid_administradorNavigations, opt => opt.Ignore())
                .ForMember(dest => dest.Vacacionno_usuarioNavigations, opt => opt.Ignore());


            // USUARIO RESPONSE
            CreateMap<Usuario, UsuarioResponseDTO>()
                .ForMember(dest => dest.NoUsuario, opt => opt.MapFrom(src => src.no_usuario))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre))
                .ForMember(dest => dest.ApPaterno, opt => opt.MapFrom(src => src.ap_paterno))
                .ForMember(dest => dest.ApMaterno, opt => opt.MapFrom(src => src.ap_materno))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.EsManager, opt => opt.MapFrom(src => src.es_manager))
                .ForMember(dest => dest.EstadoCuenta, opt => opt.MapFrom(src => src.estado_cuenta))
                .ForMember(dest => dest.DebeCambiarPass, opt => opt.MapFrom(src => src.debe_cambiar_pass))
                .ForMember(dest => dest.Intentos, opt => opt.MapFrom(src => src.intentos))
                .ForMember(dest => dest.HoraBloqueo, opt => opt.MapFrom(src => src.hora_bloqueo))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.RefreshToken))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));





            // HISTORIAL CONTRASEÑA CREATE
            CreateMap<HistorialContrasenaCreateDto, HistorialContrasena>()
                .ForMember(dest => dest.id_historial, opt => opt.Ignore())
                .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.contrasena_hash,
                    opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Contrasena)))
                .ForMember(dest => dest.fecha_registro,
                    opt => opt.MapFrom(src => DateTime.UtcNow))

                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.no_usuarioNavigation, opt => opt.Ignore());


            // HISTORIAL CONTRASEÑA PARA RESPONSE Y UPDATE
            CreateMap<HistorialContrasenaAmbosDto, HistorialContrasena>()
                .ForMember(dest => dest.id_historial, opt => opt.MapFrom(src => src.IdHistorial))
                .ForMember(dest => dest.fecha_registro, opt => opt.MapFrom(src => src.Fecharegistro))
                .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.NoUsuario))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion))
                .ForMember(dest => dest.contrasena_hash, opt => opt.Ignore())
                .ForMember(dest => dest.no_usuarioNavigation, opt => opt.Ignore());


            // HISTORIAL CONTRASEÑA RESPONSE
            CreateMap<HistorialContrasena, HistorialContrasenaAmbosDto>()
                .ForMember(dest => dest.IdHistorial, opt => opt.MapFrom(src => src.id_historial))
                .ForMember(dest => dest.Fecharegistro, opt => opt.MapFrom(src => src.fecha_registro))
                .ForMember(dest => dest.NoUsuario, opt => opt.MapFrom(src => src.no_usuario))
                .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));



            CreateMap<VacacionDto.VacacionCreateDto, Vacacion>()
                .ForMember(dest => dest.id_vacacion, opt => opt.Ignore())
                .ForMember(dest => dest.fecha_inicio, opt => opt.MapFrom(src => ToDateOnly(src.FechaInicio)))
                .ForMember(dest => dest.fecha_fin, opt => opt.MapFrom(src => ToDateOnly(src.FechaFin)))
                .ForMember(dest => dest.estado, opt => opt.MapFrom(src => "Pendiente"))
                .ForMember(dest => dest.no_usuario, opt => opt.MapFrom(src => src.SolicitanteId))
                .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
                .ForMember(dest => dest.no_usuarioNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.id_administradorNavigation, opt => opt.Ignore());

            CreateMap<VacacionDto.VacacionAprobacionDto, Vacacion>()
     .ForMember(dest => dest.id_vacacion, opt => opt.MapFrom(src => src.VacacionId))
     .ForMember(dest => dest.estado, opt => opt.MapFrom(src => src.EstadoDecision))
     .ForMember(dest => dest.fecha_inicio, opt => opt.Ignore())
     .ForMember(dest => dest.fecha_fin, opt => opt.Ignore())
     .ForMember(dest => dest.no_usuario, opt => opt.Ignore())
     .ForMember(dest => dest.no_usuarioNavigation, opt => opt.Ignore())
     .ForMember(dest => dest.id_administradorNavigation, opt => opt.Ignore());

            CreateMap<Vacacion, VacacionDto.VacacionResponseDto>()
                .ForMember(dest => dest.VacacionId, opt => opt.MapFrom(src => src.id_vacacion))
                .ForMember(dest => dest.FechaInicio, opt => opt.MapFrom(src => ToDateTime(src.fecha_inicio)))
                .ForMember(dest => dest.FechaFin, opt => opt.MapFrom(src => ToDateTime(src.fecha_fin)))
                .ForMember(dest => dest.EstadoAprobacion, opt => opt.MapFrom(src => src.estado))
                .ForMember(dest => dest.SolicitanteId, opt => opt.MapFrom(src => src.no_usuario))
                .ForMember(dest => dest.NombreSolicitante, opt => opt.MapFrom(src =>
                    src.no_usuarioNavigation != null
                        ? $"{src.no_usuarioNavigation.nombre} {src.no_usuarioNavigation.ap_paterno}".Trim()
                        : null))
                .ForMember(dest => dest.NombreAprobador, opt => opt.MapFrom(src =>
                    src.id_administradorNavigation != null
                        ? $"{src.id_administradorNavigation.nombre} {src.id_administradorNavigation.ap_paterno}".Trim()
                        : null));


            CreateMap<ProyectoDto, Proyecto>()
                .ForMember(dest => dest.codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.id_empresa, opt => opt.MapFrom(src => src.IdEmpresa))
                .ForMember(dest => dest.Registros, opt => opt.Ignore())
                .ForMember(dest => dest.id_empresaNavigation, opt => opt.Ignore());

            CreateMap<Proyecto, ProyectoDto>()
    .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.codigo))

    .ForMember(dest => dest.EsCobrable, opt => opt.MapFrom(src => src.es_cobrable))

    .ForMember(dest => dest.IdEmpresa, opt => opt.MapFrom(src => src.id_empresa));



        }

        private static DateOnly ToDateOnly(DateTime date)
        {
            return DateOnly.FromDateTime(date);
        }

        private static DateTime ToDateTime(DateOnly date)
        {
            return date.ToDateTime(TimeOnly.MinValue);
        }

        private static TimeOnly ToTimeOnly(TimeSpan time)
        {
            return TimeOnly.FromTimeSpan(time);
        }

        private static TimeSpan ToTimeSpan(TimeOnly time)
        {
            return time.ToTimeSpan();
        }
    }
}
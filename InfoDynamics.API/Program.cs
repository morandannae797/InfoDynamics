using InfoDynamics.API.Middleware;
using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.mapeo;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Aplicacion.servicios.Servicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using InfoDynamics.Infraestructura.Contexto;
using InfoDynamics.Infraestructura.Processors;
using InfoDynamics.Infraestructura.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Razor Pages
builder.Services.AddRazorPages();

// OpenAPI
builder.Services.AddOpenApi();

// Base de datos 
builder.Services.AddDbContext<EmployeesDbContext>(options =>
    options.UseInMemoryDatabase("InfoDynamicsTestDB"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// Repositorio y Unit of Work
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// === LADO DE LECTURA ===
builder.Services.AddScoped<IReadServiceAsync<EmpresaDto>, ReadServiceAsync<Empresa, EmpresaDto>>();
builder.Services.AddScoped<IReadServiceAsync<PeriodoDto>, ReadServiceAsync<Periodo, PeriodoDto>>();
builder.Services.AddScoped<IReadServiceAsync<UsuarioCreateDTO>, ReadServiceAsync<Usuario, UsuarioCreateDTO>>();
builder.Services.AddScoped<IReadServiceAsync<UsuarioResponseDTO>, ReadServiceAsync<Usuario, UsuarioResponseDTO>>();
builder.Services.AddScoped<IReadServiceAsync<VacacionDto.VacacionResponseDTO>, ReadServiceAsync<Vacacion, VacacionDto.VacacionResponseDTO>>();
builder.Services.AddScoped<IReadServiceAsync<RegistroJornadaDto>, ReadServiceAsync<Registro, RegistroJornadaDto>>();

// === LADO DE ESCRITURA ===
builder.Services.AddScoped<IWriteServiceAsync<EmpresaDto>, WriteServiceAsync<Empresa, EmpresaDto>>();
builder.Services.AddScoped<IWriteServiceAsync<PeriodoDto>, WriteServiceAsync<Periodo, PeriodoDto>>();
builder.Services.AddScoped<IWriteServiceAsync<UsuarioCreateDTO>, WriteServiceAsync<Usuario, UsuarioCreateDTO>>();
builder.Services.AddScoped<IWriteServiceAsync<VacacionDto.VacacionCreateDTO>, WriteServiceAsync<Vacacion, VacacionDto.VacacionCreateDTO>>();
builder.Services.AddScoped<IWriteServiceAsync<VacacionDto.VacacionAprobacionDTO>, WriteServiceAsync<Vacacion, VacacionDto.VacacionAprobacionDTO>>();
builder.Services.AddScoped<IWriteServiceAsync<RegistroJornadaDto>, WriteServiceAsync<Registro, RegistroJornadaDto>>();

// Servicios de dominio
builder.Services.AddScoped<Iusuarioservicio, UsuarioServicio>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
builder.Services.AddHttpContextAccessor();

// HmacServicio 
builder.Services.AddScoped<IHmacServicio, HmacServicio>();

// JWT Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JwtOptionKey));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policy =>
    {
        policy.WithOrigins("https://localhost:7293", "https://www.infodynamics.com")
              .WithHeaders(HeaderNames.Accept, HeaderNames.ContentType, HeaderNames.Authorization)
              .AllowCredentials()
              .AllowAnyMethod();
    });
});

// CORS — política dinámica desde appsettings
var cors = builder.Configuration.GetSection("Cors");
var allowedOrigins = cors.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowSubdomainsUnder = cors.GetSection("AllowSubdomainsUnder").Get<string[]>() ?? Array.Empty<string>();
var allowCredentials = cors.GetValue("AllowCredentials", false);
var maxAge = cors.GetValue("MaxAgeSeconds", 600);
var exposed = cors.GetSection("ExposedHeaders").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins);

        if (allowSubdomainsUnder.Length > 0)
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase))
                    return true;

                if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    foreach (var parent in allowSubdomainsUnder)
                    {
                        if (uri.Scheme is "https" or "http" &&
                            uri.Host.EndsWith("." + parent, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
                return false;
            });
        }

        policy.WithHeaders("Content-Type", "Authorization", "X-Request-ID")
              .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS");

        if (allowCredentials)
            policy.AllowCredentials();

        if (exposed.Length > 0)
            policy.WithExposedHeaders(exposed);

        policy.SetPreflightMaxAge(TimeSpan.FromSeconds(maxAge));
    });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration
        .GetSection(JwtOptions.JwtOptionKey)
        .Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                       System.Text.Encoding.UTF8.GetBytes(jwtOptions.Secret)),
        ValidateIssuerSigningKey = true
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["ACCESS_TOKEN"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt => opt.WithTitle("JWT + RefreshToken Auth API"));
}

app.UseCors("FrontendClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
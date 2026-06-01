using Azure.Identity;
using InfoDynamics.API.Middleware;
using InfoDynamics.Aplicacion.Abstracts;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Aplicacion.mapeo;
using InfoDynamics.Aplicacion.servicio;
using InfoDynamics.Aplicacion.servicio.IServicios;
using InfoDynamics.Aplicacion.servicios;
using InfoDynamics.Aplicacion.servicios.IServicios;
using InfoDynamics.Aplicacion.servicios.IServicios.IServicioMapping;
using InfoDynamics.Aplicacion.servicios.Servicios;
using InfoDynamics.Aplicacion.Servicios.Servicios;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using InfoDynamics.Infraestructura.Contexto;
using InfoDynamics.Infraestructura.Processors;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using InfoDynamics.Infraestructura.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Scalar.AspNetCore;
using System.Reflection.Emit;
using System.Security.Claims;
using static InfoDynamics.Aplicacion.dtos.VacacionDto;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddRazorPages();

builder.Services.AddOpenApi();



// Base de datos 
var keyVaultName = builder.Configuration["KeyVaultName"];
//que onda como andamos
if (!string.IsNullOrWhiteSpace(keyVaultName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://KeyVaultS26.vault.azure.net/"),
        new DefaultAzureCredential());
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "WARNING.");
}

builder.Services.AddDbContext<EmployeesDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    ));

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));
builder.Services.AddTransient<IEmailService, SendSmtpEmailService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});


// Repositorio y Unit of Work
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// lADO DE LECTURA 
builder.Services.AddScoped<
    IReadServiceAsync<EmpresaDto>,
    ReadServiceAsync<Empresa, EmpresaDto>>();
builder.Services.AddScoped<
    IReadServiceAsync<PeriodoDto>,
    ReadServiceAsync<Periodo, PeriodoDto>>();
builder.Services.AddScoped<
    IReadServiceAsync<UsuarioResponseDTO>,
    ReadServiceAsync<Usuario, UsuarioResponseDTO>>();
builder.Services.AddScoped<
    IReadServiceAsync<VacacionResponseDto>,
    ReadServiceAsync<Vacacion, VacacionResponseDto>>();
builder.Services.AddScoped<
    IReadServiceAsync<RegistroDto>,
    ReadServiceAsync<Registro, RegistroDto>>();
//Agregacion de proyecto
builder.Services.AddScoped<
    IReadServiceAsync<ProyectoDto>,
    ReadServiceAsync<Proyecto, ProyectoDto>>();



// lado lectura
builder.Services.AddScoped<
    IWriteServiceAsync<EmpresaDto, EmpresaDto>,
    WriteServiceAsync<Empresa, EmpresaDto, EmpresaDto>>();
//Agregacion de proyecto
builder.Services.AddScoped<
    IWriteServiceSingleAsync<ProyectoDto>,
    WriteServiceSingleAsync<Proyecto, ProyectoDto>>();

builder.Services.AddScoped<Iusuarioservicio, UsuarioServicio>();

builder.Services.AddScoped<
    IWriteServiceAsync<VacacionCreateDto, VacacionAprobacionDto>,
    WriteServiceAsync<Vacacion, VacacionCreateDto, VacacionAprobacionDto>>();

 builder.Services.AddScoped<
    IWriteServiceAsync<RegistroCreateDto, RegistroDto>,
    RegistroJornadaService > ();

builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<Iusuarioservicio, UsuarioServicio>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthTokenProcessor, AuthTokenProcessor>();
builder.Services.AddScoped<PeriodoService>();
builder.Services.AddHttpContextAccessor();

// Account
builder.Services.AddScoped<LoginValidacion>();
builder.Services.AddScoped<LoginIntentosValidacion>();


// Contraseña
builder.Services.AddScoped<IContrasenaService, ContrasenaService>();


// Empresa
builder.Services.AddScoped<EmpresaValidacionService>();
builder.Services.AddScoped<EmpresaRegistroService>();




// Jornada
builder.Services.AddScoped<RegistroJornadaService>();
builder.Services.AddScoped<JornadaValidacionService>();
builder.Services.AddScoped<JornadaRegistroService>();
builder.Services.AddScoped<JornadaCalculoService>();

//Proyeto
builder.Services.AddScoped<RegistroProyectoService>();


builder.Services.AddScoped<PeriodoService>();
builder.Services.AddScoped<IWriteServiceAsync<PeriodoDto, PeriodoDto>, WriteServiceAsync<Periodo, PeriodoDto, PeriodoDto>>();



builder.Services.AddScoped<IVacacionAprobacionService, VacacionAprobacionService>();


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JwtOptionKey));

// CORS
// JWT Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.JwtOptionKey));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policy =>
    {
        policy.WithOrigins("https://localhost:7293", "https://www.infodynamics.lat")
                .WithHeaders(HeaderNames.Accept, HeaderNames.ContentType, HeaderNames.Authorization)
              .AllowCredentials()
              .AllowAnyMethod();
    });
});


// CORS
var cors = builder.Configuration.GetSection("Cors");
var allowedOrigins = cors.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var allowSubdomainsUnder = cors.GetSection("AllowSubdomainsUnder").Get<string[]>() ?? Array.Empty<string>();
var allowCredentials = cors.GetValue("AllowCredentials", false);
var maxAge = cors.GetValue("MaxAgeSeconds", 600);
var exposed = cors.GetSection("ExposedHeaders").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontCors", policy =>
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
              .WithMethods("GET", "POST", "OPTIONS");

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

        ValidateIssuerSigningKey = true,
        RoleClaimType = ClaimTypes.Role
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
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();




app.UseExceptionHandler();
app.UseCors("FrontCors");//comentado para pruebas
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
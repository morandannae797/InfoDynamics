using InfoDynamics.Aplicacion.CustomException;
using InfoDynamics.Aplicacion.dtos;
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfoDynamics.Aplicacion.Servicios.Servicios
{
    public class EmpresaValidacionService
    {
        private readonly IGenericRepository<Empresa> _empresaRepo;

        public EmpresaValidacionService(IUnitOfWork unitOfWork)
        {
            _empresaRepo = unitOfWork.Repository<Empresa>();
        }

        public void ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new BadRequestException("El nombre de la empresa es obligatorio.");
        }

        public async Task ValidarDuplicadoAsync(string nombre)
        {
            var empresas = await _empresaRepo.GetAllAsync();

            var existe = empresas.Any(e =>
                e.nombre.ToLower() == nombre.ToLower());

            if (existe)
                throw new ConflictException("La empresa ya existe.");
        }

        public async Task ValidarDuplicadoUpdateAsync(int id, string nombre)
        {
            var empresas = await _empresaRepo.GetAllAsync();

            var existe = empresas.Any(e =>
                e.nombre.ToLower() == nombre.ToLower()
                && e.id_empresa != id);

            if (existe)
                throw new ConflictException("Ya existe otra empresa con ese nombre.");
        }

        public bool? EvaluarCodigo(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            if (codigo.StartsWith("L"))
                return true;

            if (codigo.StartsWith("M"))
                return false;

            throw new BadRequestException("El código debe iniciar con L o M.");
        }
    }

    public class EmpresaRegistroService
    {
        private readonly IGenericRepository<Empresa> _empresaRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmpresaValidacionService _validacion;

        public EmpresaRegistroService(
            IUnitOfWork unitOfWork,
            EmpresaValidacionService validacion)
        {
            _unitOfWork = unitOfWork;
            _empresaRepo = unitOfWork.Repository<Empresa>();
            _validacion = validacion;
        }

        // =====================
        // CREATE
        // =====================
        public async Task CreateAsync(EmpresaCreateDto dto)
        {
            _validacion.ValidarNombre(dto.Nombre);
            await _validacion.ValidarDuplicadoAsync(dto.Nombre);

            var empresa = new Empresa
            {
                nombre = dto.Nombre
            };

            await _empresaRepo.AddAsync(empresa);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================
        // UPDATE
        // =====================
        public async Task UpdateAsync(EmpresaDto dto)
        {
            _validacion.ValidarNombre(dto.Nombre);

            await _validacion.ValidarDuplicadoUpdateAsync(
                dto.IdEmpresa,
                dto.Nombre);

            var empresa = await _empresaRepo.GetByIdAsync(dto.IdEmpresa);

            if (empresa == null)
                throw new EntityNotFoundException("Empresa no encontrada.");

            empresa.nombre = dto.Nombre;

            try
            {
                await _empresaRepo.UpdateAsync(empresa);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException(
                    "El registro fue modificado por otro usuario.");
            }
        }
    }

    public class EmpresaAuditoriaService
    {
        // pendiente
    }
}
using InfoDynamics.Dominio.Entidades;
using InfoDynamics.Dominio.interfaces;

namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class CodeVerificacionServicio
    {
        private readonly IUnitOfWork _unitOfWork;

        public CodeVerificacionServicio(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerarYEnviarCodigoAsync(string email)
        {
            var usuarioRepo = _unitOfWork.Repository<Usuario>();
            var otpRepo = _unitOfWork.Repository<OneTimePass>();

            var usuario = await usuarioRepo.FirstOrDefaultAsync(
                u => u.email == email);

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            string codigo = GenerarOtp();

            var otpExistente = await otpRepo.FirstOrDefaultAsync(
                o => o.no_usuario == usuario.no_usuario);

            if (otpExistente == null)
            {
                await otpRepo.AddAsync(new OneTimePass
                {
                    no_usuario = usuario.no_usuario,
                    codigo = codigo,
                    fecha_caduca = DateTime.Now.AddMinutes(15)
                });
            }
            else
            {
                otpExistente.codigo = codigo;
                otpExistente.fecha_caduca = DateTime.Now.AddMinutes(15);

                await otpRepo.UpdateAsync(otpExistente);
            }

            await _unitOfWork.SaveChangesAsync();

            return codigo;
        }

        //public async Task GenerarYEnviarCodigoAsync(string email)
        //{
        //    var usuarioRepo = _unitOfWork.Repository<Usuario>();
        //    var otpRepo = _unitOfWork.Repository<OneTimePass>();

        //    var usuario = await usuarioRepo.FirstOrDefaultAsync(
        //        u => u.email == email);

        //    if (usuario == null)
        //        throw new Exception("Usuario no encontrado");

        //    string codigo = GenerarOtp();

        //    var otpExistente = await otpRepo.FirstOrDefaultAsync(
        //        o => o.no_usuario == usuario.no_usuario);

        //    if (otpExistente == null)
        //    {
        //        await otpRepo.AddAsync(new OneTimePass
        //        {
        //            no_usuario = usuario.no_usuario,
        //            codigo = codigo,
        //            fecha_caduca = DateTime.Now.AddMinutes(15)
        //        });
        //    }
        //    else
        //    {
        //        otpExistente.codigo = codigo;
        //        otpExistente.fecha_caduca = DateTime.Now.AddMinutes(15);

        //        await otpRepo.UpdateAsync(otpExistente);
        //    }

        //    await _unitOfWork.SaveChangesAsync();

        //    // TEMPORAL PARA PRUEBAS
        //    Console.WriteLine(
        //        $"OTP generado para {usuario.email}: {codigo}");
        //}

        public async Task<bool> ValidarCodigoAsync(
            int noUsuario,
            string codigoIngresado)
        {
            var otpRepo = _unitOfWork.Repository<OneTimePass>();

            var otp = await otpRepo.FirstOrDefaultAsync(
                x => x.no_usuario == noUsuario);

            if (otp == null)
                return false;

            if (CodigoExpirado(otp.fecha_caduca))
                return false;

            return otp.codigo == codigoIngresado;
        }

        private bool CodigoExpirado(DateTime? fechaCaduca)
        {
            if (!fechaCaduca.HasValue)
                return true;

            return DateTime.Now > fechaCaduca.Value;
        }

        private string GenerarOtp()
        {
            return Random.Shared
                .Next(100000, 999999)
                .ToString();
        }
    }
}
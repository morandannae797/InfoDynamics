using System.Runtime.Serialization;

namespace InfoDynamics.Aplicacion.CustomException

{

    // 404 NOT FOUND CONVERCION EN CONTROLLER
    // Se usa cuando el recurso solicitado no existe en la base de datos.
    // Ejemplo: de que empresa no encontrada o un usuario inexistente.
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() { }
        public EntityNotFoundException(string? message) : base(message) { }
        public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    // 401 UNAUTHORIZED
    // Se usa cuando el usuario no esta autenticado o no tiene permisos.
    // Ejemplo: token invalido, contraseña incorrecta, acceso denegado.
    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string? message) : base(message) { }
        public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException) { }
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    // 409 CONFLICT
    // Se usa cuando existe un conflicto de datos.
    // Ejemplo: e correo repetido, empresa duplicada, codigo ya registrado
    [Serializable]
    public class ConflictException : Exception
    {
        public ConflictException() { }
        public ConflictException(string? message) : base(message) { }
        public ConflictException(string? message, Exception? innerException) : base(message, innerException) { }
        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }


    // 400 BAD REQUEST
    // Se usa cuando los datos enviados por el cliente son invlidos.
    // Ejemplo: nombre vacío, longitud incorrecta, formato invalido.

    [Serializable]
    public class BadRequestException : Exception
    {
        public BadRequestException() { }
        public BadRequestException(string? message) : base(message) { }
        public BadRequestException(string? message, Exception? innerException) : base(message, innerException) { }
        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

}

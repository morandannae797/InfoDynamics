using System.Runtime.Serialization;

namespace InfoDynamics.Aplicacion.CustomException
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() { }
        public EntityNotFoundException(string? message) : base(message) { }
        public EntityNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }
        public UnauthorizedException(string? message) : base(message) { }
        public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException) { }
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class ConflictException : Exception
    {
        public ConflictException() { }
        public ConflictException(string? message) : base(message) { }
        public ConflictException(string? message, Exception? innerException) : base(message, innerException) { }
        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
namespace Gemona.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException() 
            : base("Você não tem permissão para acessar este recurso.")
        {
        }
    }
}

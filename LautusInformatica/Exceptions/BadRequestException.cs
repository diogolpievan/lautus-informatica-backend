namespace LautusInformatica.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException()
            : base(400, "Bad Request")
        {
        }
        public BadRequestException(string message)
            : base(400, message)
        {
        }
    }
}

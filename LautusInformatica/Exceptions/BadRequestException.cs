namespace LautusInformatica.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException()
            : base(400, "Bad Request")
        {
        }
    }
}

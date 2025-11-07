namespace LautusInformatica.Exceptions.AlreadyExists
{
    public class AlreadyExistsException : AppException
    {
        public AlreadyExistsException(string message) : base(409, message)
        {
        }
    }
}

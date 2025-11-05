namespace LautusInformatica.Exceptions.NotFound
{
    public class NotFoundException : AppException
    {
        public NotFoundException()
            : base(404, "Not Found")
        {
        }
        public NotFoundException(string message)
            : base(404, message)
        {
        }
    }
}

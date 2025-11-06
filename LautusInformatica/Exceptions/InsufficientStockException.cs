namespace LautusInformatica.Exceptions
{
    public class InsufficientStockException : AppException
    {
        public InsufficientStockException() : base(422, "Insufficient stock for the requested operation.")
        {
        }
    }
}

namespace LautusInformatica.Exceptions
{
    public class InsufficientStockException : AppException
    {
        public InsufficientStockException() : base(422, "Estoque insuficiente para esta operação")
        {
        }
    }
}

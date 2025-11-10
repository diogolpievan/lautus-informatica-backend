namespace LautusInformatica.Exceptions.BadRequest
{
    public class InvalidQuantityException : BadRequestException
    {
        public InvalidQuantityException() 
            : base("A quantidade informada é inválida.") 
        { }
    }
}

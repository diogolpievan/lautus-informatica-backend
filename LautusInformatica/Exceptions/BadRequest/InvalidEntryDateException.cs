namespace LautusInformatica.Exceptions.BadRequest
{
    public class InvalidEntryDateException : BadRequestException
    {
        public InvalidEntryDateException()
            : base("A data de entrada não pode ser maior que a data atual")
        {
        }
    }
}

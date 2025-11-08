namespace LautusInformatica.Exceptions.BadRequest
{
    public class InvalidCompletionDateException : BadRequestException
    {
        public InvalidCompletionDateException()
            : base("A data de conclusão não pode ser anterior à data de entrada")
        {
        }
    }
}

using LautusInformatica.Models.Enums;

namespace LautusInformatica.Exceptions.BadRequest
{
    public class InvalidStatusException : BadRequestException
    {
        public InvalidStatusException(string status)
            : base($"Status inválido: {status}. Valores válidos: {string.Join(", ", Enum.GetNames<Status>())}")
        {
        }
    }
}
namespace LautusInformatica.Exceptions.BadRequest
{
    public class UsedItemNotInServiceOrderException : BadRequestException
    {
        public UsedItemNotInServiceOrderException(int usedItemId, int serviceOrderId)
            : base($"Item utilizado {usedItemId} não pertence à ordem de serviço {serviceOrderId}")
        {
        }
    }
}
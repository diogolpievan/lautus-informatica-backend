namespace LautusInformatica.Exceptions.Conflict
{
    public class ServiceOrderHasUsedItemsException : ConflictException
    {
        public ServiceOrderHasUsedItemsException()
            : base("Ordem de serviço possui itens utilizados e não pode ser excluída")
        {
        }
    }
}

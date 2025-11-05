namespace LautusInformatica.Exceptions.NotFound
{
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException()
            : base("Item não encontrado")
        {
        }
    }
}

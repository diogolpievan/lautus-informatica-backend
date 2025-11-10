namespace LautusInformatica.Exceptions.NotFound
{
    public class UsedItemNotFoundException : NotFoundException
    {
        public UsedItemNotFoundException() : base("Item utilizado não encontrado")
        {
        }
    }
}

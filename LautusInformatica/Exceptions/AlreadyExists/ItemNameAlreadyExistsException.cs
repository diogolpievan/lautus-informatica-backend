namespace LautusInformatica.Exceptions.AlreadyExists
{
    public class ItemNameAlreadyExistsException : AlreadyExistsException

    {
        public ItemNameAlreadyExistsException() 
            : base("O nome do item fornecido já está em uso por outro item.") 
        { }
    }
}

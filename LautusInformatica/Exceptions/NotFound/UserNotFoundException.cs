namespace LautusInformatica.Exceptions.NotFound
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
            : base("Usuário não encontrado")
        {
        }
    }
}

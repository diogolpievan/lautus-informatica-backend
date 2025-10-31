namespace LautusInformatica.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException()
            : base(404, "Usuário não encontrado")
        {
        }
    }
}

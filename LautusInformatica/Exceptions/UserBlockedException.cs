namespace LautusInformatica.Exceptions
{
    public class UserBlockedException : AppException
    {
        public UserBlockedException()
            : base(403, "Usuário bloqueado, contate o Administrador")
        {
        }
    }
}

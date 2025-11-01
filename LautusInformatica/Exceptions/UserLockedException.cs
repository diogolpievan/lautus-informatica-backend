namespace LautusInformatica.Exceptions
{
    public class UserLockedException : AppException
    {
        public UserLockedException()
            : base(403, "Usuário bloqueado, contate o Administrador")
        {
        }
    }
}

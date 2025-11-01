namespace LautusInformatica.Exceptions
{
    public class UserEmailAlreadyExistsException : AppException
    {
        public UserEmailAlreadyExistsException() 
            : base(409, "O email fornecido já está em uso por outro usuário.") 
        { }
    }
}

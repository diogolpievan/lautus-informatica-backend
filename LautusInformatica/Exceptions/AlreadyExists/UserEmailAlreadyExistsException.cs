namespace LautusInformatica.Exceptions.AlreadyExists
{
    public class UserEmailAlreadyExistsException : AlreadyExistsException
    {
        public UserEmailAlreadyExistsException() 
            : base("O email fornecido já está em uso por outro usuário.") 
        { }
    }
}

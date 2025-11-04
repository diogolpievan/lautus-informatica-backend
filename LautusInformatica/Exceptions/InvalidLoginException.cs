namespace LautusInformatica.Exceptions
{
    public class InvalidLoginException : AppException
    {
        public InvalidLoginException() 
            : base(401, "Credenciais de login inválidas. Por favor, verifique seu email e senha.") 
        { }
    }
}

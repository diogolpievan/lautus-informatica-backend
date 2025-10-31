namespace LautusInformatica.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorMessage { get; }

        public AppException(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}

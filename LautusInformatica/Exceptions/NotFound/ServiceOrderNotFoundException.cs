namespace LautusInformatica.Exceptions.NotFound
{
    public class ServiceOrderNotFoundException : NotFoundException
    {
        public ServiceOrderNotFoundException()
            : base("Ordem de serviço não encontrada")
        {
        }
    }
}


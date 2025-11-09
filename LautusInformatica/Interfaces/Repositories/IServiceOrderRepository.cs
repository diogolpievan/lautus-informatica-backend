using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Models;
using LautusInformatica.Models.Enums;

namespace LautusInformatica.Interfaces.Repositories
{
    public interface IServiceOrderRepository
    {
        public Task<IEnumerable<ServiceOrder>?> GetAllServiceOrders();
        public Task<IEnumerable<ServiceOrder>?> GetServiceOrdersByStatus(Status status);
        public Task<ServiceOrder> GetServiceOrderById(int id);
        public Task<IEnumerable<ServiceOrder>?> GetServiceOrdersByClientId(int clientId);
        public Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByFilters(int? clientId = null, Status? status = null);
        public Task<int> CreateServiceOrder(ServiceOrder serviceOrder, int authId);
        public Task<bool> UpdateServiceOrder(ServiceOrder serviceOrder, int authId);
        public Task<bool> DeleteServiceOrder(int id, int authId);
        public Task<bool> ChangeServiceOrderStatus(int id, int statusId, int authId);
        public Task<bool> CompleteServiceOrder(int id, DateOnly completionDate, int authId);
    }
}

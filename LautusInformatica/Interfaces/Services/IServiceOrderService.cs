using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Models.Enums;

namespace LautusInformatica.Interfaces.Services
{
    public interface IServiceOrderService
    {
        public Task<IEnumerable<ServiceOrderResponseDTO>?> GetAllServiceOrders();
        public Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByStatus(Status status);
        public Task<ServiceOrderResponseDTO> GetServiceOrderById(int id);
        public Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByClientId(int clientId);
        public Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByFilters(int? clientId = null, string? status = null);
        public Task<ServiceOrderResponseDTO> CreateServiceOrder(ServiceOrderRequestDTO serviceOrderDto, int authId);
        public Task<ServiceOrderResponseDTO> UpdateServiceOrder(int id, ServiceOrderRequestDTO serviceOrderDto, int authId);
        public Task<bool> DeleteServiceOrder(int id, int authId);
        public Task<bool> ChangeServiceOrderStatus(int id, int statusId, int authId);
        public Task<bool> CompleteServiceOrder(int id, DateOnly completionDate, int authId);
        
    }
}

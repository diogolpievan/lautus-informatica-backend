using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Models.Enums;

namespace LautusInformatica.Interfaces.Services
{
    public interface IServiceOrderService
    {
        Task<IEnumerable<ServiceOrderResponseDTO>?> GetAllServiceOrders();
        Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByStatus(Status status);
        Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByClientId(int clientId);
        Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByFilters(int? clientId = null, string? status = null);

        Task<ServiceOrderDetailResponseDTO> GetServiceOrderDetailById(int id);

        Task<ServiceOrderResponseDTO> CreateServiceOrder(ServiceOrderRequestDTO serviceOrderDto, int authId);
        Task<ServiceOrderResponseDTO> UpdateServiceOrder(int id, ServiceOrderRequestDTO serviceOrderDto, int authId);
        Task<bool> DeleteServiceOrder(int id, int authId);
        Task<bool> ChangeServiceOrderStatus(int id, int statusId, int authId);
        Task<bool> CompleteServiceOrder(int id, DateOnly completionDate, int authId);
    }
}
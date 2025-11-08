using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Models.Enums;
using LautusInformatica.Exceptions.NotFound;
using MySqlConnector;
using LautusInformatica.Models;
using LautusInformatica.Exceptions.BadRequest;
using LautusInformatica.Exceptions.Conflict;

namespace LautusInformatica.Services
{
    public class ServiceOrderService : IServiceOrderService
    {
        private readonly IServiceOrderRepository _serviceOrderRepository;

        public ServiceOrderService(IServiceOrderRepository serviceOrderRepository)
        {
            _serviceOrderRepository = serviceOrderRepository;
        }

        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetAllServiceOrders()
        {
            var serviceOrders = await _serviceOrderRepository.GetAllServiceOrders();
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            return serviceOrders.Select(so => new ServiceOrderResponseDTO
            {
                Id = so.Id,
                Equipment = so.Equipment,
                Problem = so.Problem,
                Description = so.Description,
                ServicePrice = so.ServicePrice,
                EntryDate = so.EntryDate,
                CompletionDate = so.CompletionDate,
                Status = so.Status.ToString(),
                UserId = so.UserId
            });
        }

        public async Task<ServiceOrderResponseDTO> GetServiceOrderById(int id)
        {
            var serviceOrder = await _serviceOrderRepository.GetServiceOrderById(id);
            if (serviceOrder == null) throw new ServiceOrderNotFoundException();

            return new ServiceOrderResponseDTO
            {
                Id = serviceOrder.Id,
                Equipment = serviceOrder.Equipment,
                Problem = serviceOrder.Problem,
                Description = serviceOrder.Description,
                ServicePrice = serviceOrder.ServicePrice,
                EntryDate = serviceOrder.EntryDate,
                CompletionDate = serviceOrder.CompletionDate,
                Status = serviceOrder.Status.ToString(),
                UserId = serviceOrder.UserId
            };
        }
       
        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByClientId(int clientId)
        {
            var serviceOrders = await _serviceOrderRepository.GetServiceOrdersByClientId(clientId);
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            return serviceOrders.Select(so => new ServiceOrderResponseDTO
            {
                Id = so.Id,
                Equipment = so.Equipment,
                Problem = so.Problem,
                Description = so.Description,
                ServicePrice = so.ServicePrice,
                EntryDate = so.EntryDate,
                CompletionDate = so.CompletionDate,
                Status = so.Status.ToString(),
                UserId = so.UserId
            });
        }


        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByStatus(Status status)
        {
            var serviceOrders = await _serviceOrderRepository.GetServiceOrdersByStatus(status);
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            return serviceOrders.Select(so => new ServiceOrderResponseDTO
            {
                Id = so.Id,
                Equipment = so.Equipment,
                Problem = so.Problem,
                Description = so.Description,
                ServicePrice = so.ServicePrice,
                EntryDate = so.EntryDate,
                CompletionDate = so.CompletionDate,
                Status = so.Status.ToString(),
                UserId = so.UserId
            });
        }

        public async Task<ServiceOrderResponseDTO> CreateServiceOrder(ServiceOrderRequestDTO serviceOrderDto, int authId)
        {
            try
            {
                var serviceOrder = new ServiceOrder
                {
                    Equipment = serviceOrderDto.Equipment,
                    Problem = serviceOrderDto.Problem,
                    Description = serviceOrderDto.Description,
                    ServicePrice = serviceOrderDto.ServicePrice,
                    EntryDate = serviceOrderDto.EntryDate,
                    CompletionDate = serviceOrderDto.CompletionDate,
                    UserId = serviceOrderDto.UserId
                };

                int createdServiceOrderId = await _serviceOrderRepository.CreateServiceOrder(serviceOrder, authId);
                var createdServiceOrder = await GetServiceOrderById(createdServiceOrderId);

                return new ServiceOrderResponseDTO
                {
                    Id = createdServiceOrder.Id,
                    Equipment = createdServiceOrder.Equipment,
                    Problem = createdServiceOrder.Problem,
                    Description = createdServiceOrder.Description,
                    ServicePrice = createdServiceOrder.ServicePrice,
                    EntryDate = createdServiceOrder.EntryDate,
                    CompletionDate = createdServiceOrder.CompletionDate,
                    Status = createdServiceOrder.Status,
                    UserId = createdServiceOrder.UserId
                };
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000": throw new UserNotFoundException();
                    case "45004": throw new InvalidCompletionDateException();
                    case "45005": throw new InvalidEntryDateException();
                    default: throw;
                }
            }
        }

        public async Task<ServiceOrderResponseDTO> UpdateServiceOrder(int id, ServiceOrderRequestDTO serviceOrderDto, int authId)
        {
            try
            {
                var serviceOrder = new ServiceOrder
                {
                    Id = id,
                    Equipment = serviceOrderDto.Equipment,
                    Problem = serviceOrderDto.Problem,
                    Description = serviceOrderDto.Description,
                    ServicePrice = serviceOrderDto.ServicePrice,
                    EntryDate = serviceOrderDto.EntryDate,
                    CompletionDate = serviceOrderDto.CompletionDate,
                    UserId = serviceOrderDto.UserId
                };
                if(await _serviceOrderRepository.UpdateServiceOrder(serviceOrder, authId))
                {
                    var updatedServiceOrder = await GetServiceOrderById(id);
                    return new ServiceOrderResponseDTO
                    {
                        Id = updatedServiceOrder.Id,
                        Equipment = updatedServiceOrder.Equipment,
                        Problem = updatedServiceOrder.Problem,
                        Description = updatedServiceOrder.Description,
                        ServicePrice = updatedServiceOrder.ServicePrice,
                        EntryDate = updatedServiceOrder.EntryDate,
                        CompletionDate = updatedServiceOrder.CompletionDate,
                        Status = updatedServiceOrder.Status,
                        UserId = updatedServiceOrder.UserId
                    };
                }
                throw new ServiceOrderNotFoundException();
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000": throw new UserNotFoundException();
                    case "45001": throw new ServiceOrderNotFoundException();
                    case "45004": throw new InvalidCompletionDateException();
                    case "45005": throw new InvalidEntryDateException();
                    default: throw;
                }
            }
        }

        public async Task<bool> DeleteServiceOrder(int id, int authId)
        {
            try
            {
                return await _serviceOrderRepository.DeleteServiceOrder(id, authId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45001": throw new ServiceOrderNotFoundException();
                    case "45003": throw new ServiceOrderHasUsedItemsException();
                    default: throw;
                }
            }
        }

        public async Task<bool> ChangeServiceOrderStatus(int id, int statusId, int authId)
        {
            try
            {
                return await _serviceOrderRepository.ChangeServiceOrderStatus(id, statusId, authId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45001": throw new ServiceOrderNotFoundException();
                    default: throw;
                }
            }
        }

        public async Task<bool> CompleteServiceOrder(int id, DateOnly completionDate, int authId)
        {
            try
            {
                return await _serviceOrderRepository.CompleteServiceOrder(id, completionDate, authId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45001": throw new ServiceOrderNotFoundException();
                    default: throw;
                }
            }
        }
    }
}

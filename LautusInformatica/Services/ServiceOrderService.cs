using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.DTOs.UsedItem;
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
        private readonly IUsedItemsService _usedItemsService;

        public ServiceOrderService(
            IServiceOrderRepository serviceOrderRepository,
            IUsedItemsService usedItemsService)
        {
            _serviceOrderRepository = serviceOrderRepository;
            _usedItemsService = usedItemsService;
        }

        // ========== LISTAGENS (SEM USEDITEMS COMPLETOS) ==========

        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetAllServiceOrders()
        {
            var serviceOrders = await _serviceOrderRepository.GetAllServiceOrders();
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            var result = new List<ServiceOrderResponseDTO>();

            foreach (var serviceOrder in serviceOrders)
            {
                // Busca apenas a contagem, não os itens completos
                var usedItemsCount = await _usedItemsService.GetUsedItemsCountByServiceOrder(serviceOrder.Id);
                var totalCost = await CalculateTotalCost(serviceOrder);

                result.Add(new ServiceOrderResponseDTO
                {
                    Id = serviceOrder.Id,
                    Equipment = serviceOrder.Equipment,
                    Problem = serviceOrder.Problem,
                    Description = serviceOrder.Description,
                    ServicePrice = serviceOrder.ServicePrice,
                    EntryDate = serviceOrder.EntryDate,
                    CompletionDate = serviceOrder.CompletionDate,
                    Status = serviceOrder.Status.ToString(),
                    UserId = serviceOrder.UserId,
                    UsedItemsCount = usedItemsCount,
                    TotalCost = totalCost
                });
            }

            return result;
        }

        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByClientId(int clientId)
        {
            var serviceOrders = await _serviceOrderRepository.GetServiceOrdersByClientId(clientId);
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            var result = new List<ServiceOrderResponseDTO>();

            foreach (var serviceOrder in serviceOrders)
            {
                var usedItemsCount = await _usedItemsService.GetUsedItemsCountByServiceOrder(serviceOrder.Id);
                var totalCost = await CalculateTotalCost(serviceOrder);

                result.Add(new ServiceOrderResponseDTO
                {
                    Id = serviceOrder.Id,
                    Equipment = serviceOrder.Equipment,
                    Problem = serviceOrder.Problem,
                    Description = serviceOrder.Description,
                    ServicePrice = serviceOrder.ServicePrice,
                    EntryDate = serviceOrder.EntryDate,
                    CompletionDate = serviceOrder.CompletionDate,
                    Status = serviceOrder.Status.ToString(),
                    UserId = serviceOrder.UserId,
                    UsedItemsCount = usedItemsCount,
                    TotalCost = totalCost
                });
            }

            return result;
        }

        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByStatus(Status status)
        {
            var serviceOrders = await _serviceOrderRepository.GetServiceOrdersByStatus(status);
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            var result = new List<ServiceOrderResponseDTO>();

            foreach (var serviceOrder in serviceOrders)
            {
                var usedItemsCount = await _usedItemsService.GetUsedItemsCountByServiceOrder(serviceOrder.Id);
                var totalCost = await CalculateTotalCost(serviceOrder);

                result.Add(new ServiceOrderResponseDTO
                {
                    Id = serviceOrder.Id,
                    Equipment = serviceOrder.Equipment,
                    Problem = serviceOrder.Problem,
                    Description = serviceOrder.Description,
                    ServicePrice = serviceOrder.ServicePrice,
                    EntryDate = serviceOrder.EntryDate,
                    CompletionDate = serviceOrder.CompletionDate,
                    Status = serviceOrder.Status.ToString(),
                    UserId = serviceOrder.UserId,
                    UsedItemsCount = usedItemsCount,
                    TotalCost = totalCost
                });
            }

            return result;
        }

        public async Task<IEnumerable<ServiceOrderResponseDTO>?> GetServiceOrdersByFilters(int? clientId = null, string? status = null)
        {
            Status? statusEnum = null;

            if (!string.IsNullOrEmpty(status))
            {
                if (!Enum.TryParse<Status>(status, true, out Status parsedStatus))
                {
                    throw new InvalidStatusException(status);
                }
                statusEnum = parsedStatus;
            }

            var serviceOrders = await _serviceOrderRepository.GetServiceOrdersByFilters(clientId, statusEnum);
            if (serviceOrders == null || !serviceOrders.Any()) return null;

            var result = new List<ServiceOrderResponseDTO>();

            foreach (var serviceOrder in serviceOrders)
            {
                var usedItemsCount = await _usedItemsService.GetUsedItemsCountByServiceOrder(serviceOrder.Id);
                var totalCost = await CalculateTotalCost(serviceOrder);

                result.Add(new ServiceOrderResponseDTO
                {
                    Id = serviceOrder.Id,
                    Equipment = serviceOrder.Equipment,
                    Problem = serviceOrder.Problem,
                    Description = serviceOrder.Description,
                    ServicePrice = serviceOrder.ServicePrice,
                    EntryDate = serviceOrder.EntryDate,
                    CompletionDate = serviceOrder.CompletionDate,
                    Status = serviceOrder.Status.ToString(),
                    UserId = serviceOrder.UserId,
                    UsedItemsCount = usedItemsCount,
                    TotalCost = totalCost
                });
            }

            return result;
        }

        // ========== DETALHE COMPLETO (COM USEDITEMS) ==========

        public async Task<ServiceOrderDetailResponseDTO> GetServiceOrderDetailById(int id)
        {
            var serviceOrder = await _serviceOrderRepository.GetServiceOrderById(id);
            if (serviceOrder == null) throw new ServiceOrderNotFoundException();

            var usedItems = await _usedItemsService.GetUsedItemsByServiceOrder(id);
            var totalItemsCost = usedItems.Sum(ui => ui.TotalPrice);

            return new ServiceOrderDetailResponseDTO
            {
                Id = serviceOrder.Id,
                Equipment = serviceOrder.Equipment,
                Problem = serviceOrder.Problem,
                Description = serviceOrder.Description,
                ServicePrice = serviceOrder.ServicePrice,
                EntryDate = serviceOrder.EntryDate,
                CompletionDate = serviceOrder.CompletionDate,
                Status = serviceOrder.Status.ToString(),
                UserId = serviceOrder.UserId,
                UsedItems = usedItems.ToList(),
                TotalItemsCost = totalItemsCost,
                TotalCost = serviceOrder.ServicePrice + totalItemsCost
            };
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
                return await ConvertToServiceOrderResponseDTO(createdServiceOrderId);
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

                if (await _serviceOrderRepository.UpdateServiceOrder(serviceOrder, authId))
                {
                    return await ConvertToServiceOrderResponseDTO(id);
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
                    default: throw;
                }
            }
        }

        public async Task<bool> ChangeServiceOrderStatus(int id, int statusId, int authId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(Status), statusId))
                {
                    throw new InvalidStatusException(statusId.ToString());
                }

                return await _serviceOrderRepository.ChangeServiceOrderStatus(id, statusId, authId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45001": throw new ServiceOrderNotFoundException();
                    case "45006": throw new InvalidStatusException(statusId.ToString());
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
                    case "45004": throw new InvalidCompletionDateException();
                    default: throw;
                }
            }
        }

        private async Task<decimal> CalculateTotalCost(ServiceOrder serviceOrder)
        {
            var usedItems = await _usedItemsService.GetUsedItemsByServiceOrder(serviceOrder.Id);
            var totalItemsCost = usedItems.Sum(ui => ui.TotalPrice);
            return serviceOrder.ServicePrice + totalItemsCost;
        }

        private async Task<ServiceOrderResponseDTO> ConvertToServiceOrderResponseDTO(int serviceOrderId)
        {
            var serviceOrder = await _serviceOrderRepository.GetServiceOrderById(serviceOrderId);
            if (serviceOrder == null) throw new ServiceOrderNotFoundException();

            var usedItemsCount = await _usedItemsService.GetUsedItemsCountByServiceOrder(serviceOrderId);
            var totalCost = await CalculateTotalCost(serviceOrder);

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
                UserId = serviceOrder.UserId,
                UsedItemsCount = usedItemsCount,
                TotalCost = totalCost
            };
        }
    }
}
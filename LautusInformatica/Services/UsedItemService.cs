using LautusInformatica.DTOs.UsedItem;
using LautusInformatica.Exceptions;
using LautusInformatica.Exceptions.BadRequest;
using LautusInformatica.Exceptions.NotFound;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Models;
using MySqlConnector;

namespace LautusInformatica.Services
{
    public class UsedItemsService : IUsedItemsService
    {
        private readonly IUsedItemsRepository _usedItemsRepository;
        private readonly IItemService _itemService;

        public UsedItemsService(IUsedItemsRepository usedItemsRepository, IItemService itemService)
        {
            _usedItemsRepository = usedItemsRepository;
            _itemService = itemService;
        }

        public async Task ValidateUsedItemBelongsToServiceOrder(int usedItemId, int serviceOrderId)
        {
            var usedItem = await GetUsedItemById(usedItemId);
            if (usedItem.ServiceOrderId != serviceOrderId)
            {
                throw new UsedItemNotInServiceOrderException(usedItemId, serviceOrderId);
            }
        }

        public async Task<UsedItemResponseDTO> GetUsedItemById(int id)
        {
            var usedItem = await _usedItemsRepository.GetUsedItemById(id);
            if (usedItem == null) throw new UsedItemNotFoundException();

            var item = await _itemService.GetItemById(usedItem.ItemId);

            return new UsedItemResponseDTO
            {
                Id = usedItem.Id,
                ServiceOrderId = usedItem.ServiceOrderId,
                Item = item,
                Quantity = usedItem.Quantity,
                TotalPrice = item.UnitPrice * usedItem.Quantity
            };
        }

        public async Task<IEnumerable<UsedItemResponseDTO>> GetUsedItemsByServiceOrder(int serviceOrderId)
        {
            var usedItems = await _usedItemsRepository.GetUsedItemsByServiceOrder(serviceOrderId);
            var result = new List<UsedItemResponseDTO>();

            foreach (var usedItem in usedItems)
            {
                var item = await _itemService.GetItemById(usedItem.ItemId);
                result.Add(new UsedItemResponseDTO
                {
                    Id = usedItem.Id,
                    ServiceOrderId = usedItem.ServiceOrderId,
                    Item = item,
                    Quantity = usedItem.Quantity,
                    TotalPrice = item.UnitPrice * usedItem.Quantity
                });
            }

            return result;
        }

        public async Task<int> GetUsedItemsCountByServiceOrder(int serviceOrderId)
        {
            var usedItems = await _usedItemsRepository.GetUsedItemsByServiceOrder(serviceOrderId);
            return usedItems.Count();
        }

        public async Task<IEnumerable<UsedItemResponseDTO>> GetUsedItemsByItem(int itemId)
        {
            var usedItems = await _usedItemsRepository.GetUsedItemsByItem(itemId);
            var result = new List<UsedItemResponseDTO>();
            var item = await _itemService.GetItemById(itemId);

            foreach (var usedItem in usedItems)
            {
                result.Add(new UsedItemResponseDTO
                {
                    Id = usedItem.Id,
                    ServiceOrderId = usedItem.ServiceOrderId,
                    Item = item,
                    Quantity = usedItem.Quantity,
                    TotalPrice = item.UnitPrice * usedItem.Quantity
                });
            }

            return result;
        }

        public async Task<UsedItemResponseDTO> CreateUsedItem(int serviceOrderId, UsedItemRequestDTO usedItemDto, int authId)
        {
            try
            {
                var usedItem = new UsedItems
                {
                    ServiceOrderId = serviceOrderId,
                    ItemId = usedItemDto.ItemId,
                    Quantity = usedItemDto.Quantity
                };

                int createdUsedItemId = await _usedItemsRepository.CreateUsedItem(usedItem, authId);
                return await GetUsedItemById(createdUsedItemId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000": throw new ServiceOrderNotFoundException();
                    case "45001": throw new ItemNotFoundException();
                    case "45002": throw new InsufficientStockException();
                    case "45003": throw new InvalidQuantityException();
                    default: throw;
                }
            }
        }

        public async Task<UsedItemResponseDTO> UpdateUsedItem(int id, int serviceOrderId, UpdateUsedItemRequestDTO usedItemDto, int authId)
        {
            try
            {
                await ValidateUsedItemBelongsToServiceOrder(id, serviceOrderId);

                var existingUsedItem = await _usedItemsRepository.GetUsedItemById(id);
                if (existingUsedItem == null) throw new UsedItemNotFoundException();

                var updatedUsedItem = new UsedItems
                {
                    Id = id,
                    Quantity = usedItemDto.Quantity,
                    ServiceOrderId = existingUsedItem.ServiceOrderId,
                    ItemId = existingUsedItem.ItemId
                };

                if (await _usedItemsRepository.UpdateUsedItem(updatedUsedItem, authId))
                {
                    return await GetUsedItemById(id);
                }
                throw new UsedItemNotFoundException();
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000": throw new UsedItemNotFoundException();
                    case "45002": throw new InsufficientStockException();
                    case "45003": throw new InvalidQuantityException();
                    default: throw;
                }
            }
        }

        public async Task<bool> DeleteUsedItem(int id, int serviceOrderId, int authId)
        {
            try
            {
                await ValidateUsedItemBelongsToServiceOrder(id, serviceOrderId);

                return await _usedItemsRepository.DeleteUsedItem(id, authId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000": throw new UsedItemNotFoundException();
                    default: throw;
                }
            }
        }
    }
}
using LautusInformatica.DTOs.Item;
using LautusInformatica.Exceptions.NotFound;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Models;
using MySqlConnector;
using LautusInformatica.Exceptions.AlreadyExists;
using LautusInformatica.Exceptions;
using LautusInformatica.Models.Enums;


namespace LautusInformatica.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ItemResponseDTO> GetItemById(int id)
        {

            var item = await _itemRepository.GetItemById(id);
            if (item == null) throw new ItemNotFoundException();
            return new ItemResponseDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Category = item.Category
            };
        }

        public async Task<IEnumerable<ItemResponseDTO>?> GetItemsByName(string name)
        {
            var items = await _itemRepository.GetItemsByName(name);
            if (items == null || !items.Any()) return null;
            return items.Select(item => new ItemResponseDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Category = item.Category
            });
        }
        public async Task<IEnumerable<ItemResponseDTO>?> GetItemsByCategory(ItemCategory category)
        {
            var items = await _itemRepository.GetItemsByCategory(category);
            if (items == null || !items.Any()) return null;
            return items.Select(item => new ItemResponseDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Category = item.Category
            });
        }
        public async Task<IEnumerable<ItemResponseDTO>> GetAllItems()
        {
            var items = await _itemRepository.GetAllItems();
            return items.Select(item => new ItemResponseDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Category = item.Category
            });
        }
        public async Task<ItemResponseDTO> CreateItem(ItemRequestDTO itemRequestDTO, int authId)
        {
            var item = new Item
            {
                Name = itemRequestDTO.Name,
                Description = itemRequestDTO.Description,
                Quantity = itemRequestDTO.Quantity,
                UnitPrice = itemRequestDTO.UnitPrice,
                Category = itemRequestDTO.Category
            };

            try
            {
                int createdItemId = await _itemRepository.CreateItem(item, authId);
                return await GetItemById(createdItemId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45001":
                        throw new ItemNameAlreadyExistsException();
                    default:
                        throw;
                }
            }
        }
        public async Task<ItemResponseDTO> UpdateItem(int id, ItemRequestDTO itemRequestDTO, int authId)
        {
            var existingItem = await GetItemById(id);
            if (existingItem == null) throw new ItemNotFoundException();

            var item = new Item
            {
                Id = existingItem.Id,
                Name = itemRequestDTO.Name,
                Description = itemRequestDTO.Description,
                Quantity = itemRequestDTO.Quantity,
                UnitPrice = itemRequestDTO.UnitPrice,
                Category = itemRequestDTO.Category
            };
            try
            {
                await _itemRepository.UpdateItem(item, authId);
                return await GetItemById(id);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000":
                        throw new ItemNotFoundException();
                    case "45001":
                        throw new ItemNameAlreadyExistsException();
                    default:
                        throw;
                }
            }
        }
        public async Task<bool> DeleteItem(int id, int authId)
        {
            var existingItem = await GetItemById(id);
            if (existingItem == null) throw new ItemNotFoundException();

            try
            {
                return await _itemRepository.DeleteItem(id, authId);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000":
                        throw new ItemNotFoundException();
                    default:
                        throw;
                }
            }
        }
        public async Task<bool> AdjustStock(int id, int quantity, int authId, string reason)
        {
            var existingItem = await GetItemById(id);
            if (existingItem == null) throw new ItemNotFoundException();
            try
            {
                return await _itemRepository.AdjustStock(id, quantity, authId, reason);
            }
            catch (MySqlException exception)
            {
                switch (exception.SqlState)
                {
                    case "45000":
                        throw new ItemNotFoundException();
                    case "43000": 
                        throw new InsufficientStockException();
                    default:
                        throw;
                }
            }
        }
    }
}


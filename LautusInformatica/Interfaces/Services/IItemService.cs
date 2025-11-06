using LautusInformatica.DTOs.Item;
using LautusInformatica.Models.Enums;

namespace LautusInformatica.Interfaces.Services
{
    public interface IItemService
    {
        public Task<ItemResponseDTO?> GetItemById(int id);
        public Task<IEnumerable<ItemResponseDTO>?> GetItemsByName(string name);
        public Task<IEnumerable<ItemResponseDTO>?> GetItemsByCategory(ItemCategory category);
        public Task<IEnumerable<ItemResponseDTO>> GetAllItems();
        public Task<ItemResponseDTO> CreateItem(ItemRequestDTO item, int authId);
        public Task<ItemResponseDTO> UpdateItem(int id, ItemRequestDTO item, int authId);
        public Task<bool> DeleteItem(int id, int authId);
        public Task<bool> AdjustStock(int id, int quantity, int authId, string reason);
    }
}

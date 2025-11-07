using LautusInformatica.Models;
using LautusInformatica.Models.Enums;

namespace LautusInformatica.Interfaces.Repositories
{
    public interface IItemRepository
    {
        public Task<Item?> GetItemById(int id);
        public Task<IEnumerable<Item>?> GetItemsByName(string name);
        public Task<IEnumerable<Item>?> GetItemsByCategory(ItemCategory category);
        public Task<IEnumerable<Item>> GetAllItems();
        public Task<int> CreateItem(Item item, int authId);
        public Task<bool> UpdateItem(Item item, int authId);
        public Task<bool> DeleteItem(int id, int authId);
        public Task<bool> AdjustStock(int id, int quantity, int authId);

    }
}

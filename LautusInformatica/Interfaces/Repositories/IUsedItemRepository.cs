using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Models;

namespace LautusInformatica.Interfaces.Repositories
{
    public interface IUsedItemsRepository
    {
        Task<UsedItems?> GetUsedItemById(int id);
        Task<IEnumerable<UsedItems>> GetUsedItemsByServiceOrder(int serviceOrderId);
        Task<IEnumerable<UsedItems>> GetUsedItemsByItem(int itemId);
        Task<int> CreateUsedItem(UsedItems usedItem, int authId);
        Task<bool> UpdateUsedItem(UsedItems usedItem, int authId);
        Task<bool> DeleteUsedItem(int id, int authId);
    }
}
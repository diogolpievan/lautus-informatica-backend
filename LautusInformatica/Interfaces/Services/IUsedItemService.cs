using LautusInformatica.DTOs.UsedItem;

namespace LautusInformatica.Interfaces.Services
{
    public interface IUsedItemsService
    {
        Task<UsedItemResponseDTO> GetUsedItemById(int id);
        Task<IEnumerable<UsedItemResponseDTO>> GetUsedItemsByServiceOrder(int serviceOrderId);
        Task<IEnumerable<UsedItemResponseDTO>> GetUsedItemsByItem(int itemId);
        Task<UsedItemResponseDTO> CreateUsedItem(int serviceOrderId, UsedItemRequestDTO usedItemDto, int authId);
        Task<UsedItemResponseDTO> UpdateUsedItem(int id, int serviceOrderId, UpdateUsedItemRequestDTO usedItemDto, int authId);
        Task<bool> DeleteUsedItem(int id, int serviceOrderId, int authId);
        Task ValidateUsedItemBelongsToServiceOrder(int usedItemId, int serviceOrderId); // Novo método
    }
}
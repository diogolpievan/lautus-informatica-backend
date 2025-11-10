using LautusInformatica.DTOs.Item;

namespace LautusInformatica.DTOs.UsedItem
{
    public class UsedItemResponseDTO
    {
        public int Id { get; set; }
        public int ServiceOrderId { get; set; }
        public ItemResponseDTO Item { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

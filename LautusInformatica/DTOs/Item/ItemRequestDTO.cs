using LautusInformatica.Models.Enums;

namespace LautusInformatica.DTOs.Item
{
    public class ItemRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public ItemCategory Category { get; set; }
    }
}

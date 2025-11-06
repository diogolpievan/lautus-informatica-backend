namespace LautusInformatica.DTOs.Item
{
    public class ItemResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Models.Enums.ItemCategory Category { get; set; }
    }
}

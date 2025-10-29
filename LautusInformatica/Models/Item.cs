namespace LautusInformatica.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice {  get; set; }
        public ItemCategory Category { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }

        public ICollection<UsedItems> UsedItems { get; set; } = new List<UsedItems>();
    }
}

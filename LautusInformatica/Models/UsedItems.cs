namespace LautusInformatica.Models
{
    public class UsedItems
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }

        public int ServiceOrderId { get; set; }
        public ServiceOrder ServiceOrder { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int Quantity { get; set; }
    }
}

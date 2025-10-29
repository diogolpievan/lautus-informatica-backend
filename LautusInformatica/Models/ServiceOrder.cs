namespace LautusInformatica.Models
{
    public class ServiceOrder
    {
        public int Id { get; set; }
        public string Equipment { get; set; }
        public string Problem { get; set; }
        public DateOnly EntryDate { get; set; }
        public DateOnly? CompletionDate { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public bool? IsDeleted { get; set; } = false;
        public DateTime? DeletedDate { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }

        public ICollection<UsedItems> UsedItems { get; set; } = new List<UsedItems>();
    }
}

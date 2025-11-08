namespace LautusInformatica.DTOs.ServiceOrder
{
    public class ServiceOrderRequestDTO
    {
        public string Equipment { get; set; }
        public string Problem { get; set; }
        public int UserId { get; set; }
        public string? Description { get; set; }
        public decimal ServicePrice { get; set; } = 0;
        public DateOnly EntryDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? CompletionDate { get; set; }

    }
}

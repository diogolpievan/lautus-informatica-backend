namespace LautusInformatica.DTOs.ServiceOrder
{
    public class ServiceOrderResponseDTO
    {
        public int Id { get; set; }
        public string Equipment { get; set; }
        public string Problem { get; set; }
        public string Description { get; set; }
        public decimal ServicePrice { get; set; }
        public DateOnly EntryDate { get; set; }
        public DateOnly? CompletionDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}

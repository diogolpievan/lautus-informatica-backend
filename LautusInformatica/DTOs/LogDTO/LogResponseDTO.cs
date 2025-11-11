using LautusInformatica.Models.Enums;

namespace LautusInformatica.DTOs.Logs
{
    public class LogResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TableName { get; set; }
        public OperationType OperationType { get; set; }
        public string Description { get; set; }
        public DateTime OperationDate { get; set; }
    }
}

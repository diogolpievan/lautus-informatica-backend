using LautusInformatica.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace LautusInformatica.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string TableName { get; set; }
        [Range(0, 2, ErrorMessage = "Tipo de Operacao nao existe")]
        public OperationType OperationType { get; set; }
        public string Description { get; set; }
        public DateTime OperationDate { get; set; } = DateTime.Now;
    }
}

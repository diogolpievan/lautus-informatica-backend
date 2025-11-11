using LautusInformatica.DTOs.Logs;

namespace LautusInformatica.Interfaces.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogResponseDTO>?> GetAllLogs();
    }
}
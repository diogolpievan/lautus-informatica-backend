using LautusInformatica.DTOs.Logs;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Interfaces.Services;

namespace LautusInformatica.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<IEnumerable<LogResponseDTO>?> GetAllLogs()
        {
            var logs = await _logRepository.GetAllLogs();
            if (logs == null || !logs.Any()) return null;
            
            return logs.Select(log => new LogResponseDTO
            {
                Id = log.Id,
                UserId = log.UserId,
                OperationType = log.OperationType,
                TableName = log.TableName,
                OperationDate = log.OperationDate,
                Description = log.Description,
            });
        }
    }
}
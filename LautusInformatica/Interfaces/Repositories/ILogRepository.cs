using Dapper;
using LautusInformatica.Models;

namespace LautusInformatica.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task<IEnumerable<Log>> GetAllLogs();
        
    }
}
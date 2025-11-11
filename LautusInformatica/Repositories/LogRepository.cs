
using LautusInformatica.Data;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Models;
using Microsoft.EntityFrameworkCore;


namespace LautusInformatica.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly AppDbContext _context;

        public LogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Log>> GetAllLogs()
        {
            return await _context.Logs
                .ToListAsync();
        }
    }
}
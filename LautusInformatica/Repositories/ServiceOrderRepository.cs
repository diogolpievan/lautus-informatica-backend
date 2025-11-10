using Dapper;
using LautusInformatica.Data;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Models;
using LautusInformatica.Models.Enums;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LautusInformatica.Repositories
{
    public class ServiceOrderRepository : IServiceOrderRepository
    {
        private readonly AppDbContext _context;

        public ServiceOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceOrder>?> GetAllServiceOrders()
        {
            return await _context.ServiceOrders
                .Where(i => !i.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceOrder>?> GetServiceOrdersByStatus(Status status)
        {
            return await _context.ServiceOrders
                .Where(s => s.Status == status && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<ServiceOrder> GetServiceOrderById(int id)
        {
            return await _context.ServiceOrders
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<IEnumerable<ServiceOrder>?> GetServiceOrdersByClientId(int clientId)
        {
            return await _context.ServiceOrders
                .Where(s => s.UserId == clientId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> CreateServiceOrder(ServiceOrder serviceOrder, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Equipment", serviceOrder.Equipment);
            parameters.Add("@p_Problem", serviceOrder.Problem);
            parameters.Add("@p_Description", serviceOrder.Description);
            parameters.Add("@p_EntryDate", serviceOrder.EntryDate.ToDateTime(TimeOnly.MinValue));
            parameters.Add("@p_CompletionDate", serviceOrder.CompletionDate?.ToDateTime(TimeOnly.MinValue));
            parameters.Add("@p_ServicePrice", serviceOrder.ServicePrice);
            parameters.Add("@p_UserId", serviceOrder.UserId);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_ServiceOrderId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_CreateServiceOrder", parameters, commandType: System.Data.CommandType.StoredProcedure);
                int serviceOrderId = parameters.Get<int>("@p_ServiceOrderId");
                return serviceOrderId;
            }
        }

        public async Task<bool> UpdateServiceOrder(ServiceOrder serviceOrder, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", serviceOrder.Id);
            parameters.Add("@p_Equipment", serviceOrder.Equipment);
            parameters.Add("@p_Problem", serviceOrder.Problem);
            parameters.Add("@p_Description", serviceOrder.Description);
            parameters.Add("@p_EntryDate", serviceOrder.EntryDate.ToDateTime(TimeOnly.MinValue));
            parameters.Add("@p_CompletionDate", serviceOrder.CompletionDate?.ToDateTime(TimeOnly.MinValue));
            parameters.Add("@p_ServicePrice", serviceOrder.ServicePrice);
            parameters.Add("@p_UserId", serviceOrder.UserId);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_UpdateServiceOrder", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return parameters.Get<bool>("@p_Success");
            }
        }

        public async Task<bool> DeleteServiceOrder(int id, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_DeleteServiceOrder", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return parameters.Get<bool>("@p_Success");
            }
        }

        public async Task<bool> ChangeServiceOrderStatus(int id, int status, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_Status", status);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_ChangeServiceOrderStatus", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return parameters.Get<bool>("@p_Success");
            }
        }

        public async Task<bool> CompleteServiceOrder(int id, DateOnly completionDate, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_CompletionDate", completionDate.ToDateTime(TimeOnly.MinValue));
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_CompleteServiceOrder", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return parameters.Get<bool>("@p_Success");
            }
        }
        public async Task<IEnumerable<ServiceOrder>?> GetServiceOrdersByFilters(int? clientId = null, Status? status = null)
        {
            var query = _context.ServiceOrders.Where(s => !s.IsDeleted);

            if (clientId.HasValue)
                query = query.Where(s => s.UserId == clientId.Value);

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value);

            return await query.ToListAsync();
        }
    }
}

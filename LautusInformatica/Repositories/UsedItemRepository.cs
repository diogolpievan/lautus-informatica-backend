using Dapper;
using LautusInformatica.Data;
using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LautusInformatica.Repositories
{
    public class UsedItemsRepository : IUsedItemsRepository
    {
        private readonly AppDbContext _context;

        public UsedItemsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UsedItems?> GetUsedItemById(int id)
        {
            return await _context.UsedItems
                .FirstOrDefaultAsync(ui => ui.Id == id && !ui.IsDeleted);
        }

        public async Task<IEnumerable<UsedItems>> GetUsedItemsByServiceOrder(int serviceOrderId)
        {
            return await _context.UsedItems
                .Where(ui => ui.ServiceOrderId == serviceOrderId && !ui.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<UsedItems>> GetUsedItemsByItem(int itemId)
        {
            return await _context.UsedItems
                .Where(ui => ui.ItemId == itemId && !ui.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> CreateUsedItem(UsedItems usedItem, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_ServiceOrderId", usedItem.ServiceOrderId);
            parameters.Add("@p_ItemId", usedItem.ItemId);
            parameters.Add("@p_Quantity", usedItem.Quantity);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_UsedItemId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_CreateUsedItem", parameters, commandType: System.Data.CommandType.StoredProcedure);
                int usedItemId = parameters.Get<int>("@p_UsedItemId");
                return usedItemId;
            }
        }

        public async Task<bool> UpdateUsedItem(UsedItems usedItem, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", usedItem.Id);
            parameters.Add("@p_Quantity", usedItem.Quantity);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_UpdateUsedItem", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return parameters.Get<bool>("@p_Success");
            }
        }

        public async Task<bool> DeleteUsedItem(int id, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_DeleteUsedItem", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return parameters.Get<bool>("@p_Success");
            }
        }

    }
}
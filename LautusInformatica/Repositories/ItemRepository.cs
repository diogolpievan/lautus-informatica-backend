using Dapper;
using LautusInformatica.Data;
using LautusInformatica.Interfaces.Repositories;
using LautusInformatica.Models;
using LautusInformatica.Models.Enums;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace LautusInformatica.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Item?> GetItemById(int id)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }
        public async Task<IEnumerable<Item>?> GetItemsByName(string name)
        {
            return await _context.Items
                .Where(i => i.Name.Contains(name) && !i.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<Item>?> GetItemsByCategory(ItemCategory category)
        {
            return await _context.Items
                .Where(i => i.Category == category && !i.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            return await _context.Items
                .Where(i => !i.IsDeleted)
                .ToListAsync();
        }
        public async Task<int> CreateItem(Item item, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Name", item.Name);
            parameters.Add("@p_Description", item.Description);
            parameters.Add("@p_Quantity", item.Quantity);
            parameters.Add("@p_UnitPrice", item.UnitPrice);
            parameters.Add("@p_Category", item.Category);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_ItemId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_CreateItem", parameters, commandType: System.Data.CommandType.StoredProcedure);
                int userId = parameters.Get<int>("@p_ItemId");
                return userId;
            }
        }
        public async Task<bool> UpdateItem(Item item, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", item.Id);
            parameters.Add("@p_Name", item.Name);
            parameters.Add("@p_Description", item.Description);
            parameters.Add("@p_Quantity", item.Quantity);
            parameters.Add("@p_UnitPrice", item.UnitPrice);
            parameters.Add("@p_Category", item.Category);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);


            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_UpdateItem", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");

                return success;
            }
        }
        public async Task<bool> DeleteItem(int id, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);
            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_DeleteItem", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");

                return success;
            }
        }
        public async Task<bool> AdjustStock(int id, int quantity, int authId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_Id", id);
            parameters.Add("@p_Quantity", quantity);
            parameters.Add("@p_AuthId", authId);
            parameters.Add("@p_Success", dbType: System.Data.DbType.Boolean, direction: System.Data.ParameterDirection.Output);
            using (var connection = new MySqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.ExecuteAsync("sp_AdjustStock", parameters, commandType: System.Data.CommandType.StoredProcedure);
                bool success = parameters.Get<bool>("@p_Success");
                return success;
            }

        }
    }
}

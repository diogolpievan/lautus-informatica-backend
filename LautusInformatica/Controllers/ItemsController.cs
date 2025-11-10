using LautusInformatica.DTOs;
using LautusInformatica.DTOs.Item;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LautusInformatica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ItemsController : BaseController
    {
        private readonly IItemService _itemService;
        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ItemResponseDTO>>>> GetAllItems()
        {
            var items = await _itemService.GetAllItems();
            var apiResponse = new ApiResponse<IEnumerable<ItemResponseDTO>>
            {
                Message = "Items listado com sucesso",
                Success = true,
                Data = items
            };
            return Ok(apiResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ItemResponseDTO>>> GetItemById(int id)
        {
            var item = await _itemService.GetItemById(id);
            var apiResponse = new ApiResponse<ItemResponseDTO>
            {
                Message = "Item encontrado com sucesso",
                Success = true,
                Data = item
            };
            return Ok(apiResponse);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<ApiResponse<ItemResponseDTO>>> GetItemsByName(string name)
        {
            var items = await _itemService.GetItemsByName(name);
            var apiResponse = new ApiResponse<IEnumerable<ItemResponseDTO>>
            {
                Message = "Items encontrados com sucesso",
                Success = true,
                Data = items
            };
            return Ok(apiResponse);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ItemResponseDTO>>>> GetItemsByCategory(string category)
        {
            var items = await _itemService.GetItemsByCategory(Enum.Parse<ItemCategory>(category, true));
            var apiResponse = new ApiResponse<IEnumerable<ItemResponseDTO>>
            {
                Message = "Items encontrados com sucesso",
                Success = true,
                Data = items
            };
            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ItemResponseDTO>>> CreateItem([FromBody] ItemRequestDTO itemRequestDTO)
        {
            var createdItem = await _itemService.CreateItem(itemRequestDTO, UserId);
            var apiResponse = new ApiResponse<ItemResponseDTO>
            {
                Message = "Item criado com sucesso",
                Success = true,
                Data = createdItem
            };
            return CreatedAtAction(nameof(GetItemById), new { id = createdItem.Id }, apiResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ItemResponseDTO>>> UpdateItem(int id, [FromBody] ItemRequestDTO itemRequestDTO)
        {
            var updatedItem = await _itemService.UpdateItem(id, itemRequestDTO, UserId);
            var apiResponse = new ApiResponse<ItemResponseDTO>
            {
                Message = "Item atualizado com sucesso",
                Success = true,
                Data = updatedItem
            };
            return Ok(apiResponse);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteItem(int id)
        {
            var result = await _itemService.DeleteItem(id, UserId);
            var apiResponse = new ApiResponse<bool>
            {
                Message = "Item deletado com sucesso",
                Success = true,
                Data = result
            };
            return Ok(apiResponse);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<ItemResponseDTO>>> AdjustStock(int id,[FromBody] AdjustStockRequest request)
        {
            var result = await _itemService.AdjustStock(
                id,
                request.Quantity,
                UserId);

            var apiResponse = new ApiResponse<bool>
            {
                Message = "Estoque ajustado com sucesso",
                Success = true,
                Data = result
            };
            return Ok(apiResponse);
        }
    }
}

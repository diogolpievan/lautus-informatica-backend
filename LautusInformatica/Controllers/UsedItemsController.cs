using LautusInformatica.DTOs;
using LautusInformatica.DTOs.UsedItem;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Exceptions.BadRequest;
using LautusInformatica.Exceptions.NotFound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LautusInformatica.Controllers
{
    [ApiController]
    [Route("api/service-orders/{serviceOrderId}/used-items")]
    [Authorize(Roles = "Admin")]
    public class UsedItemsController : BaseController
    {
        private readonly IUsedItemsService _usedItemsService;

        public UsedItemsController(IUsedItemsService usedItemsService)
        {
            _usedItemsService = usedItemsService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsedItemResponseDTO>>>> GetUsedItemsByServiceOrder(int serviceOrderId)
        {
            var usedItems = await _usedItemsService.GetUsedItemsByServiceOrder(serviceOrderId);
            var apiResponse = new ApiResponse<IEnumerable<UsedItemResponseDTO>>
            {
                Message = "Itens utilizados listados com sucesso",
                Success = true,
                Data = usedItems
            };
            return Ok(apiResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UsedItemResponseDTO>>> GetUsedItemById(int serviceOrderId, int id)
        {
            var usedItem = await _usedItemsService.GetUsedItemById(id);

            await _usedItemsService.ValidateUsedItemBelongsToServiceOrder(id, serviceOrderId);

            var apiResponse = new ApiResponse<UsedItemResponseDTO>
            {
                Message = "Item utilizado encontrado com sucesso",
                Success = true,
                Data = usedItem
            };
            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UsedItemResponseDTO>>> CreateUsedItem(
            int serviceOrderId,
            [FromBody] UsedItemRequestDTO usedItemRequestDTO)
        {
            var createdUsedItem = await _usedItemsService.CreateUsedItem(
                serviceOrderId,
                usedItemRequestDTO,
                UserId
            );

            var apiResponse = new ApiResponse<UsedItemResponseDTO>
            {
                Message = "Item utilizado adicionado com sucesso",
                Success = true,
                Data = createdUsedItem
            };

            return CreatedAtAction(
                nameof(GetUsedItemById),
                new { serviceOrderId, id = createdUsedItem.Id },
                apiResponse
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UsedItemResponseDTO>>> UpdateUsedItem(
            int serviceOrderId,
            int id,
            [FromBody] UpdateUsedItemRequestDTO updateUsedItemRequestDTO)
        {
            var updatedUsedItem = await _usedItemsService.UpdateUsedItem(
                id,
                serviceOrderId,
                updateUsedItemRequestDTO,
                UserId
            );

            var apiResponse = new ApiResponse<UsedItemResponseDTO>
            {
                Message = "Item utilizado atualizado com sucesso",
                Success = true,
                Data = updatedUsedItem
            };

            return Ok(apiResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUsedItem(int serviceOrderId, int id)
        {
            var result = await _usedItemsService.DeleteUsedItem(
                id,
                serviceOrderId, 
                UserId
            );

            var apiResponse = new ApiResponse<bool>
            {
                Message = "Item utilizado removido com sucesso",
                Success = true,
                Data = result
            };

            return Ok(apiResponse);
        }
    }
}
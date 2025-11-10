using LautusInformatica.DTOs;
using LautusInformatica.DTOs.ServiceOrder;
using LautusInformatica.Exceptions.BadRequest;
using LautusInformatica.Interfaces.Services;
using LautusInformatica.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LautusInformatica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ServiceOrdersController : BaseController
    {
        private readonly IServiceOrderService _serviceOrderService;

        public ServiceOrdersController(IServiceOrderService serviceOrderService)
        {
            _serviceOrderService = serviceOrderService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ServiceOrderResponseDTO>>>> GetServiceOrders(
            [FromQuery] int? clientId = null,
            [FromQuery] string? status = null)
        {
            var serviceOrders = await _serviceOrderService.GetServiceOrdersByFilters(clientId, status);

            var apiResponse = new ApiResponse<IEnumerable<ServiceOrderResponseDTO>>
            {
                Message = serviceOrders != null && serviceOrders.Any()
                    ? "Ordens de serviço listadas com sucesso"
                    : "Nenhuma ordem de serviço encontrada",
                Success = true,
                Data = serviceOrders
            };

            return Ok(apiResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ServiceOrderDetailResponseDTO>>> GetServiceOrderById(int id)
        {
            var serviceOrder = await _serviceOrderService.GetServiceOrderDetailById(id);
            var apiResponse = new ApiResponse<ServiceOrderDetailResponseDTO>
            {
                Message = "Ordem de serviço encontrada com sucesso",
                Success = true,
                Data = serviceOrder
            };
            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ServiceOrderResponseDTO>>> CreateServiceOrder([FromBody] ServiceOrderRequestDTO serviceOrderRequestDTO)
        {
            var createdServiceOrder = await _serviceOrderService.CreateServiceOrder(
                serviceOrderRequestDTO,
                UserId 
            );

            var apiResponse = new ApiResponse<ServiceOrderResponseDTO>
            {
                Message = "Ordem de serviço criada com sucesso",
                Success = true,
                Data = createdServiceOrder
            };

            return CreatedAtAction(nameof(GetServiceOrderById), new { id = createdServiceOrder.Id }, apiResponse);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ServiceOrderResponseDTO>>> UpdateServiceOrder(
            int id,
            [FromBody] ServiceOrderRequestDTO serviceOrderRequestDTO)
        {
            var updatedServiceOrder = await _serviceOrderService.UpdateServiceOrder(
                id,
                serviceOrderRequestDTO,
                UserId
            );

            var apiResponse = new ApiResponse<ServiceOrderResponseDTO>
            {
                Message = "Ordem de serviço atualizada com sucesso",
                Success = true,
                Data = updatedServiceOrder
            };

            return Ok(apiResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteServiceOrder(int id)
        {
            var result = await _serviceOrderService.DeleteServiceOrder(
                id,
                UserId
            );

            var apiResponse = new ApiResponse<bool>
            {
                Message = "Ordem de serviço deletada com sucesso",
                Success = true,
                Data = result
            };

            return Ok(apiResponse);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> ChangeServiceOrderStatus(
            int id,
            [FromBody] ChangeStatusRequestDTO request)
        {
            var result = await _serviceOrderService.ChangeServiceOrderStatus(
                id,
                (int)request.Status,
                UserId
            );

            var apiResponse = new ApiResponse<bool>
            {
                Message = "Status da ordem de serviço alterado com sucesso",
                Success = true,
                Data = result
            };

            return Ok(apiResponse);
        }

        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<ApiResponse<bool>>> CompleteServiceOrder(int id)
        {
            var result = await _serviceOrderService.ChangeServiceOrderStatus(
                id,
                (int)Status.Completed,
                UserId
            );

            var apiResponse = new ApiResponse<bool>
            {
                Message = "Ordem de serviço completada com sucesso",
                Success = true,
                Data = result
            };

            return Ok(apiResponse);
        }
    }
}
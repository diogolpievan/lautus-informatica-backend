using LautusInformatica.Data;
using LautusInformatica.DTOs;
using LautusInformatica.DTOs.User;
using LautusInformatica.DTOs.Auth;
using LautusInformatica.Models;
using LautusInformatica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LautusInformatica.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using LautusInformatica.DTOs.Logs;

namespace LautusInformatica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LogsController : BaseController
    {
        private readonly ILogger<LogsController> _logger;
        private readonly ILogService _logService;

        public LogsController(ILogger<LogsController> logger, ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<LogResponseDTO>>>> GetAllLogs()
        {
            var logs = await _logService.GetAllLogs();

            var apiResponse = new ApiResponse<IEnumerable<LogResponseDTO>>
            {
                Message = "Logs listados com sucesso",
                Success = true,
                Data = logs
            };

            return Ok(apiResponse);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LautusInformatica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public abstract class BaseController : ControllerBase
    {
        protected int UserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new UnauthorizedAccessException("User ID not found in token");
                }
                return int.Parse(userIdClaim);
            }
        }
    }
}
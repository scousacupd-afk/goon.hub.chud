using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class ModerationService : ControllerBase
    {
        [HttpPost("/api/CCU")]
        public OkObjectResult CCU()
        {
            return Ok(new CCUResponse
            {
                CCUTotal = WebsocketService.ConnectedClients.Count,
                ErrorMessage = null
            });
        }
    }
}
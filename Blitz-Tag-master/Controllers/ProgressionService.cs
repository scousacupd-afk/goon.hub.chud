using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using Blitz_Tag;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class ProgressionService : ControllerBase
    {
        [HttpPost("/api/GetProgression")]
        public IActionResult GetProgression([FromBody] GetProgressionRequest request)
        {
             if (!JsonWebToken.Verify(request.MothershipToken, out var mothershipId))
             {
                return StatusCode(500); 
             }
             
             return StatusCode(500); 
        }
    }

    public class MothershipRequest
    {
        public string MothershipId { get; set; } = "";

        public string MothershipToken { get; set; } = "";

        public string MothershipEnvId { get; set; } = "";

        public string MothershipDeploymentId { get; set; } = "";
    }
    
    public class GetProgressionRequest : MothershipRequest
    {
        public string TrackId { get; set; } = "";
    }
}
using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PlayFab;
using PlayFab.ServerModels;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class SharedService : ControllerBase
    {
        [HttpPost("/api/Publish")]
        public async Task<IActionResult> GetQuestStatus([FromBody] SharedPublishMapRequestData request)
        {
            if (!JsonWebToken.Verify(request.MothershipToken, out var mothershipId) || mothershipId != request.MothershipId)
            {
                return StatusCode(500, "An error occurred on the server.");
            }

            var players = await MongoDB.Players.FindAsync(Builders<MongoDB.Player>.Filter.Eq(p => p.MothershipId, request.MothershipId));
            var player = await players.FirstOrDefaultAsync();
            if (player == null)
            {
                return BadRequest();
            }

            return Ok("CCCCCCCC");
        }
    }
}
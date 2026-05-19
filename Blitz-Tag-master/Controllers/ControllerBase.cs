using MongoDB.Driver;
using PlayFab;
using PlayFab.ServerModels;

namespace Blitz_Tag.Controllers
{
    public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
		internal static async Task<MongoDB.Player> GetMongoDBPlayerFromOculusId(string oculusId)
        {
            var player = await MongoDB.Players.Find(Builders<MongoDB.Player>.Filter.Eq(p => p.OculusId, oculusId)).FirstOrDefaultAsync();

            return player;
        }

        internal static async Task<MongoDB.Player> GetMongoDBPlayerFromPlayFabId(string playFabId)
        {
            var player = await MongoDB.Players
                .Find(Builders<MongoDB.Player>.Filter.Eq(p => p.RoomInfo.FriendLinkId, playFabId))
                .FirstOrDefaultAsync();
            
            if (player == null)
            {
                throw new Exception("yeah no");
            }
            
            return player;
        }

        // TODO: remove this void in place of a extension, or wait until MongoDB.Player.RoomInfo.FriendLinkId is safe.
        internal static FilterDefinition<MongoDB.Player> GetMongoDBPlayerFromPlayFabIdFilter(string? playFabId)
        {
            if (playFabId == null)
            {
                throw new Exception("yeah no");
            }
            
            return Builders<MongoDB.Player>.Filter.Eq(p => p.RoomInfo.FriendLinkId, playFabId);
        }
    }
}
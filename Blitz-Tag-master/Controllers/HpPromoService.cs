using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Driver;
using PlayFab;
using PlayFab.ServerModels;

namespace Blitz_Tag.Controllers
{
    public class HpPromoService : ControllerBase
    {
        [HttpPost("/api/ConsumeCodeItem")]
        public async Task<IStatusCodeActionResult> ConsumeCodeItem([FromBody] HpPromoCodeRedemptionRequest request)
        {
            if (string.IsNullOrEmpty(request.PlayFabID) || string.IsNullOrEmpty(request.ItemGUID) || string.IsNullOrEmpty(request.PlayFabSessionTicket))
            {
                return BadRequest("Missing or improperly formatted request body.");
            }

            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(
                new AuthenticateSessionTicketRequest
                {
                    SessionTicket = request.PlayFabSessionTicket
                }
            );

            if (playFabResult.Error != null || playFabResult.Result.IsSessionTicketExpired == true || playFabResult.Result.UserInfo.TitleInfo.isBanned == true || playFabResult.Result.UserInfo.PlayFabId != request.PlayFabID)
            {
                return StatusCode(500, "An error occurred on the server.");
            }

            var code = await MongoDB.Codes.Find(Builders<MongoDB.Code>.Filter.Eq(p => p.ItemGuid, request.ItemGUID)).FirstOrDefaultAsync();

            if (code == null)
            {
                return BadRequest("Invalid code.");
            }

            if (code.AlreadyRedeemed)
            {
                return Ok(new HpPromoCodeRedemptionResponse
                {
                    Result = "AlreadyRedeemed",
                    ItemID = code.ItemID,
                    PlayFabItemName = ""
                });
            }

            var updateResult =
                await MongoDB.Codes.UpdateOneAsync(
                    Builders<MongoDB.Code>.Filter.And(
                        Builders<MongoDB.Code>.Filter.Eq(p => p.ItemGuid, request.ItemGUID),
                        Builders<MongoDB.Code>.Filter.Eq(p => p.AlreadyRedeemed, false)
                    ),
                    Builders<MongoDB.Code>.Update.Set(p => p.AlreadyRedeemed, true).Set(p => p.RedeemedByUser, request.PlayFabID)
                );

            if (updateResult.ModifiedCount == 0)
            {
                return Ok(new HpPromoCodeRedemptionResponse
                {
                    Result = "AlreadyRedeemed",
                    ItemID = "",
                    PlayFabItemName = code.ItemID
                });
            }

            if (code.ItemID == "V.I.M")
            {
                var plr = await GetMongoDBPlayerFromPlayFabId(request.PlayFabID);

                await MongoDB.Players.UpdateOneAsync(
                    Builders<MongoDB.Player>.Filter.And(
                        Builders<MongoDB.Player>.Filter.Eq(p => p._id, plr._id)
                    ),
                    Builders<MongoDB.Player>.Update.Set(p => p.IsFanClub, true)
                );

                await PlayFabServerAPI.UpdateSharedGroupDataAsync(
                    new UpdateSharedGroupDataRequest
                    {
                        SharedGroupId = request.PlayFabID + "Inventory",
                        Data = new Dictionary<string, string>
                        {
                            { "subscriptions.fan_club", "{\"Sku\":\"fan_club\",\"IsActive\":true}" }
                        },
                        Permission = UserDataPermission.Public
                    }
                );

                await PlayFabServerAPI.GrantItemsToUserAsync(
                    new GrantItemsToUserRequest
                    {
                        Annotation = "Granted by automatic HpPromo system.",
                        ItemIds = [
                            "LBARC.",
                            "LBARD."
                        ],
                        PlayFabId = request.PlayFabID,
                    }
                );
            }
            else
            {
                await PlayFabServerAPI.GrantItemsToUserAsync(
                    new GrantItemsToUserRequest
                    {
                        PlayFabId = request.PlayFabID,
                        ItemIds = [.. code.ItemID.Split(",")],
                        CatalogVersion = "DLC",
                        Annotation = "Granted by automatic HpPromo system."
                    }
                );
            }

            return Ok(new HpPromoCodeRedemptionResponse
            {
                Result = "CodeRedeemed",
                ItemID = "",
                PlayFabItemName = code.ItemID
            });
        }
    }
}
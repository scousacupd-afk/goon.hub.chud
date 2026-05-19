using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class IapController : ControllerBase
    {
        [HttpPost("/api/GetMySubscriptionsAndTheirBenefits")]
        public async Task<IActionResult> GetMySubscriptionsAndTheirBenefits([FromBody] IapGetMySubscriptionsAndTheirBenefitsRequest request)
        {
            if (!JsonWebToken.Verify(request.MothershipToken, out var mothershipId))
            {
                return BadRequest(new ErrorInfo
                {
                    Message = "Invalid Mothership token",
                    Error = "InvalidMothershipToken"
                });
            }

            if (request.MothershipId != mothershipId)
            {
                return BadRequest(new ErrorInfo
                {
                    Message = "Invalid Mothership id",
                    Error = "InvalidMothershipID"
                });
            }
            
            var players = await MongoDB.Players.FindAsync<MongoDB.Player>(Builders<MongoDB.Player>.Filter.Eq(p => p.MothershipId, request.MothershipId));
            var player = await players.FirstOrDefaultAsync();

            if (player.IsFanClub == true)
            {
                return Ok(new IapGetMySubscriptionsAndTheirBenefitsResponse
                {
                    Subscriptions = [
                        new IapMothershipSubscription
                        {
                            SubscriptionId = "",
                            EarliestStartDate = DateTimeOffset.MinValue,
                            CurrentStartDate = DateTimeOffset.MinValue,
                            MostRecentBillingCycleStartDate = DateTime.Now,
                            MostRecentBillingCycleEndDate = DateTime.Now.AddYears(10),
                            TotalLifetimeSeconds = 10 * (86400 * 365),
                            IsActive = true,
                            IsCancelling = false,
                            Sku = "fan_club",
                            PlayerId = "",
                            TrialType = "",
                            ExternalServiceName = "QUEST",
                            ExternalSubscriptionId = "",
                            SubscriptionCatalogItemId = ""
                        }
                    ],
                    PreviouslyGrantedBenefitsBySubscriptionSku = [],
                    NewlyGrantedBenefitsBySubscriptionSku = [],
                    SharedGroupDataUpdateSucceeded = null
                });
            }
            
            return Ok(new IapGetMySubscriptionsAndTheirBenefitsResponse
            {
                Subscriptions = [],
                PreviouslyGrantedBenefitsBySubscriptionSku = [],
                NewlyGrantedBenefitsBySubscriptionSku = [],
                SharedGroupDataUpdateSucceeded = null
            });
        }
    }
}

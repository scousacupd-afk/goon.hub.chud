using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PlayFab;
using PlayFab.ServerModels;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class AuthenticationService : ControllerBase
    {
        [HttpPost("/api/PlayFabAuthentication")]
        public async Task<IActionResult> PlayFabAuthentication([FromBody] PlayfabAuthRequestData rjson)
        {
            try
            {
                if (string.IsNullOrEmpty(rjson.Nonce))
                {
                    Console.WriteLine("BadRequest-NoNonce");
                    return BadRequest(new ErrorInfo
                    {
                        Message = "Missing Nonce parameter",
                        Error = "BadRequest-NoNonce"
                    });
                }

                if (string.IsNullOrEmpty(rjson.Platform))
                {
                    Console.WriteLine("BadRequest-NoPlatform");
                    return BadRequest(new ErrorInfo
                    {
                        Message = "Missing Platform parameter",
                        Error = "BadRequest-NoPlatform"
                    });
                }

                if (string.IsNullOrEmpty(rjson.OculusId))
                {
                    Console.WriteLine("BadRequest-NoOculusId");
                    return BadRequest(new ErrorInfo
                    {
                        Message = "Missing OculusId parameter",
                        Error = "BadRequest-NoOculusId"
                    });
                }

                if (rjson.Platform != "1" && rjson.Platform != "Quest")
                {
                    Console.WriteLine("BadRequest-InvalidPlatform");
                    return BadRequest(new ErrorInfo
                    {
                        Message = $"Invalid Platform parameter: {rjson.Platform}",
                        Error = "BadRequest-InvalidPlatform"
                    });
                }

                if (string.IsNullOrEmpty(rjson.MothershipToken))
                {
                    Console.WriteLine("BadRequest-NoMothershipToken");
                    return BadRequest(new ErrorInfo
                    {
                        Message = "Missing MothershipToken parameter",
                        Error = "BadRequest-NoMothershipToken"
                    });
                }

                if (string.IsNullOrEmpty(rjson.MothershipId))
                {
                    Console.WriteLine("BadRequest-NoMothershipId");
                    return BadRequest(new ErrorInfo
                    {
                        Message = "Missing MothershipId parameter",
                        Error = "BadRequest-NoMothershipId"
                    });
                }

                if (!JsonWebToken.Verify(rjson.MothershipToken, out var mothershipId))
                {
                    Console.WriteLine("InvalidMothershipToken");
                    return BadRequest(new ErrorInfo
                    {
                        Message = "Invalid Mothership token",
                        Error = "InvalidMothershipToken"
                    });
                }

                var ua = Request.Headers.UserAgent.ToString();
                var unity = Request.Headers["X-Unity-Version"].ToString();

                if (!Constants.AllowedUserAgents.Contains(ua) || !Constants.AllowedUnityVersions.Contains(unity))
                {
                    return NotFound();
                }

                var (IsValid, Json) = await OculusServer.VerifyNonceWithJsonAsync(rjson.OculusId, rjson.Nonce);
                if (!IsValid)
                {
                    Console.WriteLine("InvalidNonce");
                    return BadRequest(new ErrorInfo
                    {
                        Message = $"Nonce was provided, but is not valid (response: {Json}) nonce: {rjson.Nonce}",
                        Error = "InvalidNonce"
                    });
                }

                OculusServer.MetaUser? graphUser = await OculusServer.GetUserAsync(rjson.OculusId);

                if (graphUser == null)
                {
                    return BadRequest(new ErrorInfo
                    {
                        Message = "how the FUCK did you bypass nonce",
                        Error = "InvalidOculusId"
                    });
                }

                if (graphUser.UserId != rjson.OculusId) // this shit means its the orgscoped id
                {
                    return BadRequest(new ErrorInfo
                    {
                        Message = "maybe try ur appscoped not ur orgscoped",
                        Error = "InvalidOculusId"
                    });
                }

                PlayFabResult<ServerLoginResult> loginResult = await PlayFabServerAPI.LoginWithCustomIDAsync(new LoginWithCustomIDRequest
                {
                    CustomId = "OCULUS" + graphUser.OrgScopedId,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                    {
                        GetUserAccountInfo = true
                    },
                    CreateAccount = true
                });

                if (loginResult.Error != null)
                {
                    if (loginResult.Error.Error == PlayFabErrorCode.AccountBanned)
                    {
                        string latestBanKey = "";
                        DateTime latestBanExpiration = DateTime.MinValue;

                        foreach (KeyValuePair<string, string[]> kvp in loginResult.Error.ErrorDetails)
                        {
                            foreach (string banTimeStr in kvp.Value)
                            {
                                if (DateTime.TryParse(banTimeStr, out var banTime))
                                {
                                    if (banTime > latestBanExpiration)
                                    {
                                        latestBanExpiration = banTime;
                                        latestBanKey = kvp.Key;
                                    }
                                }
                            }
                        }

                        Console.WriteLine("AccountBanned");
                        return StatusCode(403, new Models.BanInfo
                        {
                            BanMessage = latestBanKey,
                            BanExpirationTime = latestBanExpiration.ToString("o")
                        });
                    }

                    if (loginResult.Error.Error == PlayFabErrorCode.EvaluationModePlayerCountExceeded)
                    {
                        Console.WriteLine("EvaluationModePlayerCountExceeded");
                        return StatusCode(403, new Models.BanInfo
                        {
                            BanMessage = "TOO MANY PLAYERS IN PLAYFAB!\nMESSAGE AN OWNER IMMEDIATELY.",
                            BanExpirationTime = "Indefinite"
                        });
                    }

                    return StatusCode(403);
                }
                else if (loginResult.Result.NewlyCreated)
                {
                    await PlayFabServerAPI.LinkServerCustomIdAsync(new LinkServerCustomIdRequest
                    {
                        ServerCustomId = "OCULUS" + graphUser.OrgScopedId,
                        PlayFabId = loginResult.Result.PlayFabId,
                        ForceLink = true
                    });
                }

                ServerLoginResult data = loginResult.Result;

                return Ok(new PlayfabAuthResponseData
                {
                    SessionTicket = data.SessionTicket,
                    EntityToken = data.EntityToken.EntityToken,
                    PlayFabId = data.PlayFabId,
                    EntityId = data.EntityToken.Entity.Id,
                    EntityType = data.EntityToken.Entity.Type,
                    AccountCreationIsoTimestamp = data.InfoResultPayload.AccountInfo.Created,
                    FriendCode = ""
                });
            }
            catch (Exception ex)
            {
                // yes gtag actually returns this
                // curl https://auth-prod.gtag-cf.com/api/PlayFabAuthentication --header "User-Agent: UnityPlayer" --data "{'Nonce': '0','OculusId': '0','Platform': '3'}" -X POST
                // if you want to replicate

                Console.WriteLine(ex);
                return StatusCode(500, new ErrorInfo
                {
                    Message = "Failed",
                    Error = "Exception"
                });
            }
        }

        [HttpPost("/api/PhotonAuthentication")]
        public async Task<IActionResult> PhotonAuthentication([FromBody] PhotonAuthRequestData request, [FromQuery] bool? IsVoice)
        {
            if (!JsonWebToken.Verify(request.MothershipToken))
            {
                return Ok(new
                {
                    ResultCode = 0
                });
            }

            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.Ticket
            });

            if (playFabResult.Result.IsSessionTicketExpired == true || playFabResult.Result.UserInfo.TitleInfo.isBanned == true)
            {
                return Ok(new
                {
                    ResultCode = 0
                });
            }

            List<UpdateDefinition<MongoDB.Player>> updates = [];

            if (request.IsPublic != null)
            {
                updates.Add(Builders<MongoDB.Player>.Update.Set(p => p.RoomInfo.IsPublic, request.IsPublic));
            }

            if (!string.IsNullOrEmpty(request.Zone))
            {
                updates.Add(Builders<MongoDB.Player>.Update.Set(p => p.RoomInfo.Zone, request.Zone));
            }

            if (updates.Count > 0)
            {
                await MongoDB.Players.UpdateOneAsync(
                    Builders<MongoDB.Player>.Filter.Eq(p => p.OrgScopedId, playFabResult.Result.UserInfo.ServerCustomIdInfo.CustomId.Split("OCULUS")[1]),
                    Builders<MongoDB.Player>.Update.Combine(updates)
                );
            }

            return Ok(new
            {
                ResultCode = 1,
                UserId = playFabResult.Result.UserInfo.PlayFabId
            });
        }
    }
}
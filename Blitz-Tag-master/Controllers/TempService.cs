using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PlayFab;
using PlayFab.ServerModels;
using System.Globalization;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class TempService : ControllerBase
    {
        [HttpPost("/api/GetQuestStatus")]
        public async Task<IActionResult> GetQuestStatus([FromBody] TempGetQuestStatusRequestData request)
        {
            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Error != null)
            {
                return BadRequest();
            }

            var players = await MongoDB.Players.FindAsync(Builders<MongoDB.Player>.Filter.Eq(p => p.RoomInfo.FriendLinkId, playFabResult.Result.UserInfo.PlayFabId));
            var player = await players.FirstOrDefaultAsync();
            if (player == null)
            {
                return BadRequest();
            }

            return Ok(new TempGetQuestStatusResponseData
            {
                Result = player.QuestStatus
            });
        }

        [HttpPost("/api/SetQuestComplete")]
        public async Task<IActionResult> SetQuestComplete([FromBody] TempGetQuestStatusRequestData request)
        {
            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Error != null)
            {
                return BadRequest();
            }

            var players = await MongoDB.Players.FindAsync(Builders<MongoDB.Player>.Filter.Eq(p => p.RoomInfo.FriendLinkId, playFabResult.Result.UserInfo.PlayFabId));
            var player = await players.FirstOrDefaultAsync();
            if (player == null)
            {
                return BadRequest();
            }

            var questStatus = player.QuestStatus;
            var today = DateTime.UtcNow.Date;
            var weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var todayKey = today.ToString("MM/dd/yyyy");

            questStatus.DailyPoints = questStatus.DailyPoints
                .Where(d => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Parse(d.Key), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == weekNumber)
                .ToDictionary(d => d.Key, d => d.Value);

            if (!questStatus.WeeklyPoints.TryGetValue(weekNumber, out int currentWeekPoints))
            {
                currentWeekPoints = 0;
                questStatus.WeeklyPoints[weekNumber] = currentWeekPoints;
            }

            if (currentWeekPoints + 1 > 25)
            {
                return StatusCode(403, "Completing quest would exceed weekly point limit");
            }

            if (!questStatus.DailyPoints.ContainsKey(todayKey))
            {
                questStatus.DailyPoints[todayKey] = 0;
            }

            questStatus.DailyPoints[todayKey] += 1;
            questStatus.WeeklyPoints[weekNumber] = questStatus.DailyPoints.Values.Sum();
            questStatus.UserPointsTotal = questStatus.DailyPoints.Values.Sum();

            var update = Builders<MongoDB.Player>.Update.Set(p => p.QuestStatus, questStatus);
            await MongoDB.Players.UpdateOneAsync(Builders<MongoDB.Player>.Filter.Eq(p => p.RoomInfo.FriendLinkId, player.RoomInfo.FriendLinkId), update);

            return Ok(new TempGetQuestStatusResponseData
            {
                Result = questStatus
            });
        }

        [HttpPost("/api/GetFriendsV2")]
        public async Task<IActionResult> GetFriendsV2([FromBody] TempGetFriendsRequestData request)
        {
            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Error != null)
            {
                return BadRequest();
            }

            var player = await GetMongoDBPlayerFromPlayFabId(playFabResult.Result.UserInfo.PlayFabId);

            List<TempFriendResponseData> friends = [];

            foreach (var friendLinkId in player.Friends)
            {
                var friend = await GetMongoDBPlayerFromPlayFabId(friendLinkId);
                if (!friend.Friends.Contains(player.RoomInfo.FriendLinkId) || !player.Friends.Contains(friend.RoomInfo.FriendLinkId))
                {
                    continue;
                }

                switch (friend.PrivacyState)
                {
                    case PrivacyState.VISIBLE: 
                        friends.Add(new TempFriendResponseData // player is visible so we always add all the data
                        { 
                            Presence = friend.RoomInfo,
                            Created = DateTime.UtcNow 
                        });
                    break;

                    case PrivacyState.PUBLIC_ONLY:
                        if (friend.RoomInfo.IsPublic == true || friend.RoomInfo.IsPublic == null)
                        {
                            friends.Add(new TempFriendResponseData // players privacy state is PUBLIC_ONLY and they are not in a room/the room is public, so add all data
                            {
                                Presence = friend.RoomInfo,
                                Created = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            friends.Add(new TempFriendResponseData // players privacy state is PUBLIC_ONLY, but the room they are in is not public. 
                            {
                                Presence = new TempFriendPresenceResponseData
                                {
                                    FriendLinkId = friend.RoomInfo.FriendLinkId,
                                    UserName = friend.RoomInfo.UserName,
                                    Region = null,
                                    IsPublic = null,
                                    RoomId = null,
                                    Zone = null
                                },
                                Created = DateTime.UtcNow
                            });
                        }
                        break;

                    case PrivacyState.HIDDEN: 
                        friends.Add(new TempFriendResponseData // players privacy state is HIDDEN, so just return friend link id and username!
                        {
                            Presence = new TempFriendPresenceResponseData
                            {
                                FriendLinkId = friend.RoomInfo.FriendLinkId,
                                UserName = friend.RoomInfo.UserName,
                                Region = null,
                                IsPublic = null,
                                RoomId = null,
                                Zone = null
                            },
                            Created = DateTime.UtcNow
                        });
                    break;
                }
            }

            return Ok(new TempGetFriendsResponseData
            {
                Result = new TempGetFriendsResultData
                {
                    Friends = friends,
                    MyPrivacyState = player.PrivacyState
                },
                StatusCode = 200,
                Error = null
            });
        }

        [HttpPost("/api/RequestFriend")]
        public async Task<IActionResult> RequestFriend([FromBody] TempAddFriendRequestData request)
        {
            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Error != null || request.MyFriendLinkId != request.PlayFabId || playFabResult.Result.UserInfo.PlayFabId != request.PlayFabId)
            {
                return BadRequest();
            }

            var update = Builders<MongoDB.Player>.Update.AddToSet(p => p.Friends, request.FriendFriendLinkId); // adds only if not present

            await MongoDB.Players.UpdateOneAsync(GetMongoDBPlayerFromPlayFabIdFilter(playFabResult.Result.UserInfo.PlayFabId), update);

            return Ok();
        }

        [HttpPost("/api/RemoveFriend")]
        public async Task<IActionResult> RemoveFriend([FromBody] TempRemoveFriendRequestData request)
        {
            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Error != null || request.MyFriendLinkId != request.PlayFabId || playFabResult.Result.UserInfo.PlayFabId != request.PlayFabId)
            {
                return BadRequest();
            }

            var update = Builders<MongoDB.Player>.Update.Pull(p => p.Friends, request.FriendFriendLinkId); // pulls only if present

            await MongoDB.Players.UpdateOneAsync(GetMongoDBPlayerFromPlayFabIdFilter(playFabResult.Result.UserInfo.PlayFabId), update);

            return Ok();
        }

        [HttpPost("/api/SetPrivacyState")]
        public async Task<IActionResult> SetPrivacyState([FromBody] TempSetPrivacyStateRequestData request)
        {
            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Error != null || !Enum.TryParse<PrivacyState>(request.PrivacyState, ignoreCase: true, out var privacyStateEnum))
            {
                return BadRequest();
            }
            
            await MongoDB.Players.UpdateOneAsync(GetMongoDBPlayerFromPlayFabIdFilter(playFabResult.Result.UserInfo.PlayFabId), Builders<MongoDB.Player>.Update.Set(p => p.PrivacyState, PrivacyState.VISIBLE));

            return Ok();
        }
    }
}
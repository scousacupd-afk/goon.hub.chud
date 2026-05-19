using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Driver;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ServerModels;
using System.Security.Cryptography;
using System.Text;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class FunctionService : ControllerBase
    {
        public static class Function
        {
            public static string ReportButtonNames(int button)
            {
                return button switch
                {
                    0 => "HATE SPEECH",
                    1 => "CHEATING",
                    2 => "TOXICITY",
                    _ => "NOT ASSIGNED",
                };
            }

            // TODO: make instaban in mongodb

            public static readonly Dictionary<string, InstabanUser> insta = new()
            {
                { 
                    "", 
                    new InstabanUser() {
                        BanTime = TimeSpan.FromDays(7),
                        Username = "BXT"
                    } 
                },
                { 
                    "", 
                    new InstabanUser() {
                        BanTime = TimeSpan.FromDays(30),
                        Username = "NEVA"
                    } 
                }
            };

            public class InstabanUser
            {
                public string Username { get; set; } = "";

                public TimeSpan BanTime { get; init; }
            }
        }

        [HttpPost("/api/CheckForBadName")]
        public async Task<IActionResult> CheckForBadName([FromBody] PlayfabFunctionRequestData<PlayfabCheckForBadNameRequestData> request)
        {
            // TODO: add fuzzywuzzy to the project and use it to detect if names are very close to bad names
            // (example: F4GG0T would be close to FAGGOT, so you would detect and ban)
            // and add more names.

            if (request.FunctionArgument == null)
            {
                return BadRequest();
            }

            var isBadName = request.FunctionArgument.Name == "FAGGOT" || request.FunctionArgument.Name == "NIGGER" || request.FunctionArgument.Name == "NIGGA";

            if (isBadName)
            {
                return Ok(new
                {
                    result = CheckForBadNameResult.Ok,
                    banLength = -1
                });
            }
            
            await PlayFabServerAPI.BanUsersAsync(new BanUsersRequest
            {
                Bans = [
                    new BanRequest {
                        PlayFabId = request.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                        DurationInHours = 24,
                        Reason = "BAD MONKE"
                    }
                ]
            });
            
            // TODO: add discord webhook logs (use the bool isBadName to detect if its a bad name, if it is add 2 fields, one ban time, and second )

            return Ok(new
            {
                result = CheckForBadNameResult.Ban,
                banLength = 24
            });
        }

        [HttpPost("/api/TryDistributeCurrencyV2")]
        public async Task<OkResult> TryDistributeCurrencyV2([FromBody] PlayfabFunctionRequestData rjson)
        {
            PlayFabResult<GetUserDataResult> readOnlyDataResult = await PlayFabServerAPI.GetUserReadOnlyDataAsync(
                new GetUserDataRequest
                {
                    PlayFabId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                    Keys = ["DailyLogin"]
                }
            );

            var currentDate = DateTime.UtcNow.Date;
            var isFirstLogin = false;
            bool shouldReward;

            if (readOnlyDataResult.Result.Data?.TryGetValue("DailyLogin", out var dailyLogin) == true && DateTime.TryParse(dailyLogin?.Value, out var lastLoginDate))
            {
                shouldReward = lastLoginDate.Date != currentDate;
            }
            else
            {
                isFirstLogin = true;
                shouldReward = false;
            }

            if (isFirstLogin)
            {
                await PlayFabServerAPI.AddUserVirtualCurrencyAsync(
                    new AddUserVirtualCurrencyRequest
                    {
                        PlayFabId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                        VirtualCurrency = "SR",
                        Amount = 50000
                    }
                );

                await PlayFabServerAPI.UpdateUserReadOnlyDataAsync(
                    new UpdateUserDataRequest
                    {
                        PlayFabId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                        Data = new Dictionary<string, string>
                        {
                            { "DailyLogin", currentDate.ToString("yyyy-MM-dd") }
                        }
                    }
                );
            }

            if (shouldReward)
            {
                return Ok();
            }
            
            await PlayFabServerAPI.AddUserVirtualCurrencyAsync(
                new AddUserVirtualCurrencyRequest
                {
                    PlayFabId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                    VirtualCurrency = "SR",
                    Amount = 1000
                }
            );

            await PlayFabServerAPI.UpdateUserReadOnlyDataAsync(
                new UpdateUserDataRequest
                {
                    PlayFabId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                    Data = new Dictionary<string, string>
                    {
                        { "DailyLogin", currentDate.ToString("yyyy-MM-dd") }
                    }
                }
            );

            return Ok();
        }

        [HttpPost("/api/UpdatePersonalCosmeticsList")]
        public async Task<OkResult> UpdatePersonalCosmeticsList([FromBody] PlayfabFunctionRequestData rjson)
        {
            var getSharedGroupData = await PlayFabServerAPI.GetSharedGroupDataAsync(new GetSharedGroupDataRequest
            {
                SharedGroupId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId + "Inventory"
            });

            if (getSharedGroupData.Error != null)
            {
                if (getSharedGroupData.Error?.Error == PlayFabErrorCode.InvalidSharedGroupId)
                {
                    await PlayFabServerAPI.CreateSharedGroupAsync(new CreateSharedGroupRequest
                    {
                        SharedGroupId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId + "Inventory"
                    });
                }
            }

            var inventoryResult = await PlayFabServerAPI.GetUserInventoryAsync(
                new GetUserInventoryRequest
                {
                    PlayFabId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId
                }
            );

            StringBuilder concatItems = new();

            if (inventoryResult.Result.Inventory != null)
            {
                foreach (var item in inventoryResult.Result.Inventory)
                {
                    concatItems.Append(item.ItemId).Append(',');
                }
            }

            await PlayFabServerAPI.UpdateSharedGroupDataAsync(
                new UpdateSharedGroupDataRequest
                {
                    SharedGroupId = rjson.CallerEntityProfile.Lineage.MasterPlayerAccountId + "Inventory",
                    Data = new Dictionary<string, string>
                    {
                        { "Inventory", concatItems.ToString().TrimEnd(',') }
                    },
                    Permission = UserDataPermission.Public
                }
            );

            return Ok();
        }

        [HttpPost("/api/AddOrRemoveDLCOwnershipV2")]
        public OkResult AddOrRemoveDLCOwnershipV2()
        {
            return Ok();
        }

        [HttpPost("/api/ReturnCurrentVersionV2")]
        public OkObjectResult ReturnCurrentVersionV2()
        {
            return Ok(new PlayfabReturnCurrentVersionV2ResponseData
            {
                ResultCode = 0,
                BannedUsers = "1",
                MOTD = "SERVER MAINTENANCE HAS COMPLETED! I WOULD SAY PLEASE RETURN TO YOUR MONKEY BUSINESS, BUT I ASSUME NOBODY REALLY STOPPED.",
                QueueStats = new PlayfabReturnCurrentVersionV2TopTroopsResponseData
                {
                    TopTroops = []
                }
            });
        }

        [HttpPost("/api/ReturnMyOculusHashV2")]
        public async Task<OkObjectResult> ReturnMyOculusHashV2([FromBody] PlayfabFunctionRequestData request)
        {
            PlayFabResult<GetUserAccountInfoResult> getUserAccountInfo = await PlayFabServerAPI.GetUserAccountInfoAsync(new GetUserAccountInfoRequest
            {
                PlayFabId = request.CallerEntityProfile.Lineage.MasterPlayerAccountId
            });

            return Ok(new
            {
                oculusHash = SHA256.HashData(Encoding.ASCII.GetBytes(getUserAccountInfo.Result.UserInfo.CustomIdInfo.CustomId.Split("OCULUS")[1]))
            });
        }

        [HttpPost("/api/BroadcastMyRoomV2")]
        public async Task<OkResult> BroadcastMyRoomV2([FromBody] PlayfabFunctionRequestData<PlayfabBroadcastMyRoomV2RequestData> request)
        {
            if (request.FunctionArgument == null)
            {
                return Ok();
            }

            if (request.FunctionArgument.Set)
            {
                await PlayFabServerAPI.CreateSharedGroupAsync(new CreateSharedGroupRequest
                {
                    SharedGroupId = request.CallerEntityProfile.Lineage.MasterPlayerAccountId
                });
                await PlayFabServerAPI.UpdateSharedGroupDataAsync(new UpdateSharedGroupDataRequest
                {
                    SharedGroupId = request.CallerEntityProfile.Lineage.MasterPlayerAccountId,
                    Data = new Dictionary<string, string>
                    {
                        { request.FunctionArgument.KeyToFollow, request.FunctionArgument.RoomToJoin }
                    }
                });
            }
            else
            {
                await PlayFabServerAPI.DeleteSharedGroupAsync(new DeleteSharedGroupRequest
                {
                    SharedGroupId = request.CallerEntityProfile.Lineage.MasterPlayerAccountId
                });
            }

            return Ok();
        }

        [HttpPost("/api/GetAcceptedAgreements")]
        public IStatusCodeActionResult GetAcceptedAgreements([FromBody] PlayfabFunctionRequestData<string> request)
        {
            if (request.FunctionArgument == null)
            {
                return StatusCode(500);
            }

            PlayfabAgreementsData resp = new();
            var huh = request.FunctionArgument.Split(',');

            if (huh.Contains("TOS"))
            {
                resp.TOS = "2025.06.18";
            }

            if (huh.Contains("PrivacyPolicy"))
            {
                resp.PrivacyPolicy = "2025.06.18";
            }

            return Ok(resp);
        }

        [HttpPost("/api/SubmitAcceptedAgreements")]
        public IResult SubmitAcceptedAgreements([FromBody] PlayfabFunctionRequestData<PlayfabAgreementsData> request)
        {
            if (request.FunctionArgument is not { TOS: "2025.06.18", PrivacyPolicy: "2025.06.18" }) 
            {
                return Results.InternalServerError();
            }

            return Results.Text("Agreements submitted successfully");
        }

        [HttpPost("/api/Gorillanalytics")]
        public IStatusCodeActionResult Gorillanalytics()
        {
            return Ok();
        }

        [HttpPost("/api/GetRandomName")]
        public IResult GetRandomName()
        {
            return Results.Text("gorilla" + Random.Shared.Next(1, 10000).ToString("D4"));
        }

        [HttpPost("/api/ReturnQueueStats")]
        public IStatusCodeActionResult ReturnQueueStats([FromBody] PlayfabFunctionRequestData<PlayfabReturnQueueStatsRequestData> request)
        {
            if (request.FunctionArgument == null)
            {
                return StatusCode(500);
            }

            return Ok(new PlayfabReturnQueueStatsResponseData
            {
                QueueName = request.FunctionArgument.QueueName,
                PlayerCount = 1
            });
        }

        // webhooks

        [HttpPost("/api/PhotonWebhookHandlers")]
        public async Task<OkObjectResult> PhotonWebhookHandlers([FromBody] PhotonWebhookRequest request)
        {
            PlayFabResult<GetUserAccountInfoResult> playFabResult = await PlayFabServerAPI.GetUserAccountInfoAsync(new GetUserAccountInfoRequest
            {
                PlayFabId = request.UserId
            });

            if (playFabResult.Error != null || (playFabResult.Result.UserInfo.TitleInfo.isBanned ?? true))
            {
                return Ok(new
                {
                    ResultCode = 1
                });
            }

            switch (request.Type)
            {
                case "ClientDisconnect":
                case "ClientTimeoutDisconnect":
                case "ManagedDisconnect":
                case "ServerDisconnect":
                case "TimeoutDisconnect":
                case "LeaveRequest":
                case "PlayerTtlTimedOut":
                case "PeerLastTouchTimedOut":
                case "PluginRequest":
                case "PluginFailedJoin":
                    MongoDB.Room playerRoom = await MongoDB.Rooms.Find(r => r.Players.ContainsKey(request.UserId ?? "")).FirstOrDefaultAsync();
                    if (playerRoom != null)
                    {
                        playerRoom.Players.Remove(request.UserId ?? "");
                        if (playerRoom.Players.Count == 0)
                        {
                            await MongoDB.Rooms.DeleteOneAsync(r => r.GameId == playerRoom.GameId);
                        }
                        else
                        {
                            await MongoDB.Rooms.ReplaceOneAsync(r => r.GameId == playerRoom.GameId, playerRoom);
                        }

                        await Requests.Post<string>("", new DiscordWebhook
                        {
                            Embeds = [
                                new DiscordEmbed
                                {
                                    Title = "Room Left",
                                    Fields = [
                                        new EmbedField { Name = "🔒 Code", Value = $"```{request.GameId}```", Inline = true },
                                        new EmbedField { Name = "🌐 Region", Value = $"```{request.Region}```", Inline = true },
                                        new EmbedField { Name = "🔎 Nickname", Value = $"```{request.Nickname}```", Inline = true },
                                        new EmbedField { Name = "🔗 PlayFab ID", Value = $"```{request.UserId}```", Inline = true },
                                        new EmbedField { Name = "⭐ Actor Number", Value = $"```{request.ActorNr}```", Inline = true },
                                        new EmbedField { Name = "🌲 Map", Value = $"```{playerRoom.Map}```", Inline = true },
                                        new EmbedField { Name = "🦆 Queue", Value = $"```{playerRoom.Queue}```", Inline = true }
                                    ]
                                }
                            ],
                            AllowedMentions = new AllowedMentions
                            {
                                Parse = []
                            }
                        });
                    }

                    await MongoDB.Players.UpdateOneAsync(
                        GetMongoDBPlayerFromPlayFabIdFilter(request.UserId),
                        Builders<MongoDB.Player>.Update
                        .Set(p => p.RoomInfo.RoomId, null)
                        .Set(p => p.RoomInfo.Region, null)
                        .Set(p => p.RoomInfo.IsPublic, null)
                        .Set(p => p.RoomInfo.Zone, null)
                        .Set(p => p.RoomInfo.UserName, request.Nickname)
                    );

                    break;
                case "Create":
                    string queue = "UNKNOWN";
                    string map = "UNKNOWN";

                    CustomProperties? customProperties = request.CreateOptions?.CustomProperties;
                    if (customProperties != null)
                    {
                        if (!string.IsNullOrEmpty(customProperties.QueueName))
                        {
                            queue = customProperties.QueueName;
                            string gameModeString = customProperties.GameMode ?? "UNKNOWN";
                            map = gameModeString.Split([queue], StringSplitOptions.None)[0].Replace("|", "").ToUpper();
                        }
                        else
                        {
                            var gameModeString = customProperties.GameMode ?? "UNKNOWN";

                            if (gameModeString.Contains("DEFAULT")) queue = "DEFAULT";
                            else if (gameModeString.Contains("COMPETITIVE")) queue = "COMPETITIVE";
                            else if (gameModeString.Contains("MINIGAMES")) queue = "MINIGAMES";
                            else queue = "UNKNOWN";

                            map = gameModeString.Split([queue], StringSplitOptions.None)[0].ToUpper();
                        }
                    }

                    MongoDB.Room newRoom = new()
                    {
                        GameId = request.GameId ?? "",
                        Region = request.Region ?? "",
                        Queue = queue,
                        Map = map,
                        Players = new Dictionary<string, int>
                        {
                            { request.UserId ?? "", request.ActorNr ?? 0 }
                        }
                    };

                    await MongoDB.Rooms.InsertOneAsync(newRoom);

                    await MongoDB.Players.UpdateOneAsync(
                        GetMongoDBPlayerFromPlayFabIdFilter(request.UserId),
                        Builders<MongoDB.Player>.Update
                        .Set(p => p.RoomInfo.RoomId, request.GameId)
                        .Set(p => p.RoomInfo.Region, request.Region)
                        .Set(p => p.RoomInfo.FriendLinkId, request.UserId)
                        .Set(p => p.RoomInfo.UserName, request.Nickname)
                    );

                    await Requests.Post<string>("", new DiscordWebhook
                    {
                        Embeds = [
                            new DiscordEmbed
                            {
                                Title = "Room Created",
                                Fields = [
                                    new EmbedField { Name = "🔒 Code", Value = $"```{request.GameId}```", Inline = true },
                                    new EmbedField { Name = "🌐 Region", Value = $"```{request.Region}```", Inline = true },
                                    new EmbedField { Name = "🔎 Nickname", Value = $"```{request.Nickname}```", Inline = true },
                                    new EmbedField { Name = "🔗 PlayFab ID", Value = $"```{request.UserId}```", Inline = true },
                                    new EmbedField { Name = "⭐ Actor Number", Value = $"```{request.ActorNr}```", Inline = true },
                                    new EmbedField { Name = "🌲 Map", Value = $"```{map}```", Inline = true },
                                    new EmbedField { Name = "🦆 Queue", Value = $"```{queue}```", Inline = true }
                                ]
                            }
                        ],
                        AllowedMentions = new AllowedMentions
                        {
                            Parse = []
                        }
                    });

                    break;
                case "Join":
                    if (!string.IsNullOrEmpty(request.GameId))
                    {
                        var roomFilter = Builders<MongoDB.Room>.Filter.Eq(r => r.GameId, request.GameId);
                        var existingRoom = await MongoDB.Rooms.Find(roomFilter).FirstOrDefaultAsync();

                        if (existingRoom != null)
                        {
                            existingRoom.Players[request.UserId ?? ""] = request.ActorNr ?? 0;

                            await MongoDB.Rooms.ReplaceOneAsync(roomFilter, existingRoom);

                            await MongoDB.Players.UpdateOneAsync(
                                GetMongoDBPlayerFromPlayFabIdFilter(request.UserId),
                                Builders<MongoDB.Player>.Update
                                .Set(p => p.RoomInfo.RoomId, existingRoom.GameId)
                                .Set(p => p.RoomInfo.Region, existingRoom.Region)
                                .Set(p => p.RoomInfo.FriendLinkId, request.UserId)
                                .Set(p => p.RoomInfo.UserName, request.Nickname)
                            );

                            await Requests.Post<string>("", new DiscordWebhook
                            {
                                Embeds = [
                                    new DiscordEmbed
                                    {
                                        Title = "Room Joined",
                                        Fields = [
                                            new EmbedField { Name = "🔒 Code", Value = $"```{request.GameId}```", Inline = true },
                                            new EmbedField { Name = "🌐 Region", Value = $"```{request.Region}```", Inline = true },
                                            new EmbedField { Name = "🔎 Nickname", Value = $"```{request.Nickname}```", Inline = true },
                                            new EmbedField { Name = "🔗 PlayFab ID", Value = $"```{request.UserId}```", Inline = true },
                                            new EmbedField { Name = "⭐ Actor Number", Value = $"```{request.ActorNr}```", Inline = true },
                                            new EmbedField { Name = "🌲 Map", Value = $"```{existingRoom.Map}```", Inline = true },
                                            new EmbedField { Name = "🦆 Queue", Value = $"```{existingRoom.Queue}```", Inline = true }
                                        ]
                                    }
                                ],
                                AllowedMentions = new AllowedMentions
                                {
                                    Parse = []
                                }
                            });
                        }
                    }

                    break;
                case "Event":
                    switch (request.EvCode)
                    {
                        case 10:
                            PlayFabResult<GetSharedGroupDataResult> getSharedGroupData = await PlayFabServerAPI.GetSharedGroupDataAsync(new GetSharedGroupDataRequest
                            {
                                SharedGroupId = request.UserId + "Inventory"
                            });

                            if (getSharedGroupData.Error != null)
                            {
                                if (getSharedGroupData.Error.Error == PlayFabErrorCode.InvalidSharedGroupId)
                                {
                                    await PlayFabServerAPI.CreateSharedGroupAsync(new CreateSharedGroupRequest
                                    {
                                        SharedGroupId = request.UserId + "Inventory"
                                    });
                                }
                            }

                            PlayFabResult<GetUserInventoryResult> inventoryResult = await PlayFabServerAPI.GetUserInventoryAsync(
                                new GetUserInventoryRequest
                                {
                                    PlayFabId = request.UserId
                                }
                            );

                            StringBuilder concatItems = new();

                            if (inventoryResult.Result.Inventory != null)
                            {
                                foreach (var item in inventoryResult.Result.Inventory)
                                {
                                    concatItems.Append(item.ItemId).Append(',');
                                }
                            }

                            await PlayFabServerAPI.UpdateSharedGroupDataAsync(
                                new UpdateSharedGroupDataRequest
                                {
                                    SharedGroupId = request.UserId + "Inventory",
                                    Data = new Dictionary<string, string>
                                    {
                                        { "Inventory", concatItems.ToString().TrimEnd(',') }
                                    },
                                    Permission = UserDataPermission.Public
                                }
                            );
                            break;

                        case 50:
                            if (!Function.insta.ContainsKey(request.UserId ?? ""))
                            {
                                var room = await MongoDB.Rooms.Find(r => r.GameId == request.GameId).FirstOrDefaultAsync();

                                var reason = -1;

                                if (request.Data != null && request.Data.Count > 1 && request.Data[1] != null)
                                {
                                    reason = Convert.ToInt32(request.Data[1]);
                                }

                                await Requests.Post<string>("", new DiscordWebhook
                                {
                                    Embeds = [
                                        new DiscordEmbed
                                        {
                                            Title = "Player Report",
                                            Fields = [
                                                new EmbedField { Name = "🔎 Who Reported", Value = $"```{request.Nickname}```", Inline = true },
                                                new EmbedField { Name = "🔢 Player ID", Value = $"```{request.UserId}```", Inline = true },
                                                new EmbedField { Name = "❓ Reason", Value = $"```{Function.ReportButtonNames(reason)}```", Inline = true },
                                                new EmbedField { Name = "🔎 Reported Player", Value = $"```{request.Data?[2]?.ToString() ?? ""}```", Inline = true },
                                                new EmbedField { Name = "🔢 Reported Player ID", Value = $"```{request.Data?[0]?.ToString() ?? ""}```", Inline = true },
                                                new EmbedField { Name = "🦆 Queue", Value = $"```{room?.Queue ?? "Unknown"}```", Inline = true },
                                                new EmbedField { Name = "🌲 Map", Value = $"```{room?.Map ?? "Unknown"}```", Inline = true },
                                                new EmbedField { Name = "🌐 Region", Value = $"```{request.Region}```", Inline = true },
                                            ]
                                        }
                                    ],
                                    AllowedMentions = new AllowedMentions
                                    {
                                        Parse = []
                                    }
                                });
                            }
                            else
                            {
                                var plr = Function.insta[request.UserId ?? ""];

                                var eventroom = await MongoDB.Rooms.Find(r => r.GameId == request.GameId).FirstOrDefaultAsync();

                                var reason = -1;

                                if (request.Data != null && request.Data.Count > 1 && request.Data[1] != null)
                                {
                                    reason = Convert.ToInt32(request.Data[1]);
                                }

                                var years = plr.BanTime.Days / 365;
                                var months = (plr.BanTime.Days % 365) / 30;
                                var days = (plr.BanTime.Days % 365) % 30;
                                var hours = plr.BanTime.Hours;

                                List<string> parts = [];
                                if (years > 0) parts.Add($"{years} year{(years > 1 ? "s" : "")}");
                                if (months > 0) parts.Add($"{months} month{(months > 1 ? "s" : "")}");
                                if (days > 0) parts.Add($"{days} day{(days > 1 ? "s" : "")}");
                                if (hours > 0) parts.Add($"{hours} hour{(hours > 1 ? "s" : "")}");

                                var formatted = string.Join(" ", parts);

                                await Requests.Post<string>("", new DiscordWebhook
                                {
                                    Embeds = [
                                        new DiscordEmbed
                                        {
                                            Title = "Staff Report",
                                            Fields = [
                                                new EmbedField { Name = "🔎 Who Reported", Value = $"```{request.Nickname}```", Inline = true },
                                                new EmbedField { Name = "🔢 Player ID", Value = $"```{request.UserId}```", Inline = true },
                                                new EmbedField { Name = "❓ Reason", Value = $"```{Function.ReportButtonNames(reason)}```", Inline = true },
                                                new EmbedField { Name = "🔎 Reported Player", Value = $"```{request.Data?[2]?.ToString() ?? ""}```", Inline = true },
                                                new EmbedField { Name = "🔢 Reported Player ID", Value = $"```{request.Data?[0]?.ToString() ?? ""}```", Inline = true },
                                                new EmbedField { Name = "❓ How Long?", Value = $"```{formatted}```", Inline = true },
                                                new EmbedField { Name = "🦆 Queue", Value = $"```{eventroom?.Queue ?? "Unknown"}```", Inline = true },
                                                new EmbedField { Name = "🌲 Map", Value = $"```{eventroom?.Map ?? "Unknown"}```", Inline = true },
                                                new EmbedField { Name = "🌐 Region", Value = $"```{request.Region}```", Inline = true },
                                                new EmbedField { Name = "🔒 Code", Value = $"```{request.GameId}```", Inline = true },
                                            ]
                                        }
                                    ],
                                    AllowedMentions = new AllowedMentions
                                    {
                                        Parse = []
                                    }
                                });

                                await PlayFabServerAPI.BanUsersAsync(new BanUsersRequest
                                {
                                    Bans = [
                                        new BanRequest {
                                            PlayFabId = request.Data?[0]?.ToString() ?? "",
                                            DurationInHours = (uint)plr.BanTime.TotalHours,
                                            Reason = Function.ReportButtonNames(reason) + ". ID: " + request.Data?[0]
                                        }
                                    ]
                                });
                            }
                            break;
                    }
                    break;
            }

            return Ok(new
            {
                ResultCode = 0,
                Message = "Success"
            });
        }

        public class PhotonWebhookRequest
        {
            public string? Type { get; set; }

            public string? GameId { get; set; }

            public string? Region { get; set; }

            public string? Nickname { get; set; }

            public string? UserId { get; set; }

            public int? ActorNr { get; set; }

            public int? EvCode { get; set; }

            public List<object?>? Data { get; set; }

            public CreateOptions? CreateOptions { get; set; }
        }

        public class CreateOptions
        {
            public CustomProperties? CustomProperties { get; set; }
        }

        public class CustomProperties
        {
            [JsonProperty(PropertyName = "queueName")]
            public string? QueueName { get; set; }

            [JsonProperty(PropertyName = "gameMode")]
            public string? GameMode { get; set; }
        }

        public class PlayfabReturnCurrentVersionV2ResponseData
        {
            [JsonProperty(PropertyName = "ResultCode")]
            public int ResultCode { get; set; }

            [JsonProperty(PropertyName = "BannedUsers")]
            public string BannedUsers { get; set; } = "1";

            [JsonProperty(PropertyName = "MOTD")]
            public string MOTD { get; set; } = "";

            [JsonProperty(PropertyName = "QueueStats")]
            public PlayfabReturnCurrentVersionV2TopTroopsResponseData QueueStats { get; set; } = new PlayfabReturnCurrentVersionV2TopTroopsResponseData
            {
                TopTroops = []
            };
        }

        public class PlayfabReturnCurrentVersionV2TopTroopsResponseData
        {
            [JsonProperty(PropertyName = "TopTroops")]
            public List<string> TopTroops { get; set; } = [];
        }

        public class PlayfabReturnQueueStatsRequestData
        {
            [JsonProperty(PropertyName = "QueueName")]
            public string QueueName { get; set; } = "";
        }

        public class PlayfabReturnQueueStatsResponseData : PlayfabReturnQueueStatsRequestData
        {
            [JsonProperty(PropertyName = "PlayerCount")]
            public int PlayerCount { get; set; }
        }

        public class PlayfabAgreementsData
        {
            [JsonProperty(PropertyName = "TOS", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string TOS { get; set; } = "";

            [JsonProperty(PropertyName = "PrivacyPolicy", DefaultValueHandling = DefaultValueHandling.Ignore)]
            public string PrivacyPolicy { get; set; } = "";
        }
    }
}

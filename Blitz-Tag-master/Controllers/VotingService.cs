using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PlayFab;
using PlayFab.ServerModels;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class VotingService : ControllerBase
    {
        [HttpPost("/api/FetchPoll")]
        public async Task<IActionResult> GetQuestStatus([FromBody] VotingFetchPollsRequest request)
        {
            if (string.IsNullOrEmpty(request.TitleId))
            {
                return NotFound("Must supply a valid poll title ID");
            }

            if (string.IsNullOrEmpty(request.PlayFabId))
            {
                return BadRequest("Must supply a valid user ID");
            }

            if (string.IsNullOrEmpty(request.PlayFabTicket))
            {
                return BadRequest("Must supply a valid PlayFab Ticket");
            }

            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Result.UserInfo.PlayFabId != request.PlayFabId || playFabResult.Result.IsSessionTicketExpired == true || playFabResult.Result.UserInfo.TitleInfo.isBanned == true)
            {
                return StatusCode(500);
            }

            if (request.TitleId != Constants.TitleId)
            {
                return Ok(new List<VotingFetchPollsResponse>());
            }

            List<VotingFetchPollsResponse> polls = [];

            foreach (var poll in await MongoDB.Polls.Find(FilterDefinition<MongoDB.Poll>.Empty).ToListAsync())
            {
                var isActive = DateTime.UtcNow >= poll.StartTime && DateTime.UtcNow <= poll.EndTime;
                
                if (request.IncludeInactive || isActive)
                {
                    polls.Add(new VotingFetchPollsResponse
                    {
                        PollId = poll.PollId,
                        Question = poll.Question,
                        VoteOptions = poll.VoteOptions,
                        VoteCount = isActive ? [] : poll.VoteCount,
                        PredictionCount = isActive ? [] : poll.PredictionCount,
                        StartTime = poll.StartTime,
                        EndTime = poll.EndTime,
                        IsActive = isActive
                    });
                }
            }

            return Ok(polls);
        }

        [HttpPost("/api/Vote")]
        public async Task<IActionResult> SetQuestComplete([FromBody] VotingVoteRequest request)
        {
            if (request.PollId == null)
            {
                return BadRequest("Must supply a valid poll ID");
            }

            if (request.PollId < 0)
            {
                return StatusCode(500);
            }

            if (request.TitleId == null)
            {
                return BadRequest("Must supply a valid title ID");
            }

            if (request.PlayFabId == null)
            {
                return BadRequest("Must supply a valid user ID");
            }

            if (request.UserPlatform != "Quest" && request.UserPlatform != "Steam" && request.UserPlatform != "PC")
            {
                return BadRequest("Invalid or empty platform");
            }

            if (!await OculusServer.VerifyNonceAsync(request.OculusId, request.UserNonce))
            {
                return BadRequest("Invalid nonce");
            }

            PlayFabResult<AuthenticateSessionTicketResult> playFabResult = await PlayFabServerAPI.AuthenticateSessionTicketAsync(new AuthenticateSessionTicketRequest
            {
                SessionTicket = request.PlayFabTicket
            });

            if (playFabResult.Result.UserInfo.PlayFabId != request.PlayFabId || playFabResult.Result.IsSessionTicketExpired == true || playFabResult.Result.UserInfo.TitleInfo.isBanned == true)
            {
                return StatusCode(500);
            }

            var poll = await MongoDB.Polls.Find(p => p.PollId == request.PollId).FirstOrDefaultAsync();

            if (poll == null)
            {
                return StatusCode(500, "Could not get poll from DB");
            }

            if (request.OptionIndex < 0 || request.OptionIndex >= poll.VoteOptions.Count)
            {
                return BadRequest("Invalid option index");
            }

            if (DateTime.UtcNow < poll.StartTime || DateTime.UtcNow > poll.EndTime)
            {
                return BadRequest("Poll not active");
            }

            var updateResult = await MongoDB.Players.UpdateOneAsync(
                p => p.OculusId == request.OculusId && !p.Votes.Any(v => v.PollId == request.PollId),
                Builders<MongoDB.Player>.Update.Push(p => p.Votes, new MongoDB.Vote
                {
                    PollId = request.PollId.Value,
                    OptionIndex = request.OptionIndex,
                    IsPrediction = request.IsPrediction
                })
            );

            if (updateResult.ModifiedCount == 0)
            {
                return StatusCode(429);
            }

            if (request.IsPrediction)
            {
                await MongoDB.Polls.UpdateOneAsync(
                    p => p.PollId == request.PollId,
                    Builders<MongoDB.Poll>.Update.Inc($"PredictionCount.{request.OptionIndex}", 1)
                );
            }
            else
            {
                await MongoDB.Polls.UpdateOneAsync(
                    p => p.PollId == request.PollId,
                    Builders<MongoDB.Poll>.Update.Inc($"VoteCount.{request.OptionIndex}", 1)
                );
            }

            return Ok(new VotingVoteResponse
            {
                PollId = request.PollId,
                TitleId = Constants.TitleId,
                VoteOptions = poll.VoteOptions,
                VoteCount = [],
                PredictionCount = []
            });
        }
    }
}
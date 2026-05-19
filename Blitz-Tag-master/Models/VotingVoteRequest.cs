namespace Blitz_Tag.Models
{
    public class VotingVoteRequest
    {
        public int? PollId { get; set; } = null;

        public string? TitleId { get; set; } = "";

        public string? PlayFabId { get; set; } = "";

        public string OculusId { get; set; } = "";

        public string? UserNonce { get; set; } = "";

        public string? UserPlatform { get; set; } = "";

        public int OptionIndex { get; set; } = 0;

        public bool IsPrediction { get; set; } = false;

        public string PlayFabTicket { get; set; } = "";
    }
}
namespace Blitz_Tag.Models
{
    public class VotingFetchPollsRequest
    {
        public string TitleId { get; set; } = "";

        public string PlayFabId { get; set; } = "";

        public string PlayFabTicket { get; set; } = "";

        public bool IncludeInactive { get; set; } = false;
    }
}

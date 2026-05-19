using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class VotingVoteResponse
    {
        [JsonProperty(PropertyName = "pollId")]
        public int? PollId { get; set; } = 0;

        [JsonProperty(PropertyName = "titleId")]
        public string TitleId { get; set; } = "";

        [JsonProperty(PropertyName = "voteOptions")]
        public List<string> VoteOptions { get; set; } = [];

        [JsonProperty(PropertyName = "voteCount")]
        public List<int> VoteCount { get; set; } = [];

        [JsonProperty(PropertyName = "predictionCount")]
        public List<int> PredictionCount { get; set; } = [];
    }
}
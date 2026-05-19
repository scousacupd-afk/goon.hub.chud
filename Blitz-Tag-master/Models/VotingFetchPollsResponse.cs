using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class VotingFetchPollsResponse
    {
        [JsonProperty(PropertyName = "pollId")]
        public int PollId { get; set; } = 0;

        [JsonProperty(PropertyName = "question")]
        public string Question { get; set; } = "";

        [JsonProperty(PropertyName = "voteOptions")]
        public List<string> VoteOptions { get; set; } = [];

        [JsonProperty(PropertyName = "voteCount")]
        public List<int> VoteCount { get; set; } = [];

        [JsonProperty(PropertyName = "predictionCount")]
        public List<int> PredictionCount { get; set; } = [];

        [JsonProperty(PropertyName = "startTime")]
        public DateTime StartTime { get; set; } = DateTime.MinValue;

        [JsonProperty(PropertyName = "endTime")]
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; } = false;
    }
}

using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class TempSetQuestCompleteRequestData
	{
		[JsonProperty(PropertyName = "PlayFabId")]
		public string? PlayFabId { get; set; }
		
		[JsonProperty(PropertyName = "PlayFabTicket")]
		public string? PlayFabTicket { get; set; }
		
		[JsonProperty(PropertyName = "MothershipId")]
		public string? MothershipId { get; set; }
		
		[JsonProperty(PropertyName = "MothershipToken")]
		public string? MothershipToken { get; set; }
		
		[JsonProperty(PropertyName = "QuestId")]
		public int QuestId { get; set; }
		
		[JsonProperty(PropertyName = "ClientVersion")]
		public string? ClientVersion { get; set; }
	}
}
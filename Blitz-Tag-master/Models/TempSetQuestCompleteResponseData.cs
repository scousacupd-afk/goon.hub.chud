using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class TempSetQuestCompleteResponseData
	{
		[JsonProperty(PropertyName = "result")]
		public TempUserQuestStatusResponseData Result { get; set; } = new();
	}
}

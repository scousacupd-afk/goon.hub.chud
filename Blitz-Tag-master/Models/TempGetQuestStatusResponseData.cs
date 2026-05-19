using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class TempGetQuestStatusResponseData
	{
		[JsonProperty(PropertyName = "result")]
		public TempUserQuestStatusResponseData Result { get; set; } = new();
	}
}

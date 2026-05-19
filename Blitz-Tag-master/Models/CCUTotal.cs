using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class CCUResponse
	{
		[JsonProperty(PropertyName = "ccuTotal")]
		public int CCUTotal { get; set; }

		[JsonProperty(PropertyName = "errorMessage")]
		public string? ErrorMessage { get; set; }
	}
}
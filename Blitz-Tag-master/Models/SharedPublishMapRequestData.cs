using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class SharedPublishMapRequestData : MothershipRequestBase
    {
        [JsonProperty(PropertyName = "userdataMetadataKey")]
        public string UserdataMetadataKey { get; set; } = "";

        [JsonProperty(PropertyName = "playerNickname")]
        public string PlayerNickname { get; set; } = "";
    }
}

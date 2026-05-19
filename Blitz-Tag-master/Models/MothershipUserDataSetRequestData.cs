using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipUserDataSetRequestData
    {
        [JsonProperty(PropertyName = "key_name")]
        [BsonElement("key_name")]
        public string KeyName { get; set; } = "";
        
        [JsonProperty(PropertyName = "value")]
        [BsonElement("value")]
        public string Value { get; set; } = "";
        
        [JsonProperty(PropertyName = "generation")]
        [BsonElement("generation")]
        public int Generation { get; set; }
    }
}
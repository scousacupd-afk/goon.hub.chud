using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class MothershipClientDataResponseData
    {
        [JsonProperty(PropertyName = "users")]
        public List<MothershipClientDataUser> Admins { get; set; } = [];
    }

    public class MothershipClientDataFailureResponseData
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; } = "";

        [JsonProperty(PropertyName = "errorCode")]
        public MothershipClientDataError Error { get; set; } = MothershipClientDataError.Unknown;
    }

    public class MothershipClientDataUser
    {
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; } = "";

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; } = "";

        [JsonProperty(PropertyName = "role")]
        public MothershipClientDataRole Role { get; set; } = MothershipClientDataRole.None;
    }

    public enum MothershipClientDataRole
    {
        None = -1,
        Moderator,
        Admin,
        Owner
    }

    public enum MothershipClientDataError
    {
        Unknown = -1,
        NewUpdate
    }
}

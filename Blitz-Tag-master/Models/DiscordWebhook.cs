using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
    public class DiscordWebhook
    {
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string? Username { get; set; }

        [JsonProperty("avatar_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? AvatarUrl { get; set; }

        [JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Tts { get; set; }

        [JsonProperty("embeds", NullValueHandling = NullValueHandling.Ignore)]
        public List<DiscordEmbed>? Embeds { get; set; }

        [JsonProperty("allowed_mentions", NullValueHandling = NullValueHandling.Ignore)]
        public AllowedMentions? AllowedMentions { get; set; }
    }

    public class AllowedMentions
    {
        [JsonProperty("parse", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? Parse { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? Roles { get; set; }

        [JsonProperty("users", NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? Users { get; set; }

        [JsonProperty("replied_user", NullValueHandling = NullValueHandling.Ignore)]
        public bool? RepliedUser { get; set; }
    }

    public class DiscordEmbed
    {
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; } = "rich";

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string? Url { get; set; }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public int? Color { get; set; }

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedFooter? Footer { get; set; } = null;

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedImage? Image { get; set; }

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedThumbnail? Thumbnail { get; set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedVideo? Video { get; set; }

        [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedProvider? Provider { get; set; }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedAuthor? Author { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public List<EmbedField>? Fields { get; set; }
    }
    
    public class EmbedFooter
    {
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string? Text { get; set; }

        [JsonProperty("icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? IconUrl { get; set; }

        [JsonProperty("proxy_icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? ProxyIconUrl { get; set; }
    }

    public class EmbedImage
    {
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string? Url { get; set; }

        [JsonProperty("proxy_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? ProxyUrl { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }
    }

    public class EmbedThumbnail : EmbedImage { }

    public class EmbedVideo : EmbedImage { }

    public class EmbedProvider
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string? Url { get; set; }
    }

    public class EmbedAuthor
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string? Url { get; set; }

        [JsonProperty("icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? IconUrl { get; set; }
        
        [JsonProperty("proxy_icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string? ProxyIconUrl { get; set; }
    }

    public class EmbedField
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string? Value { get; set; }

        [JsonProperty("inline", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Inline { get; set; }
    }
}

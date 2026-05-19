using MongoDB.Bson.Serialization.Attributes;

namespace Blitz_Tag.Models
{
	public class TempFriendPresenceResponseData
	{
		[BsonElement("friendLinkId")]
		public string FriendLinkId { get; set; } = "";
		
		[BsonElement("userName")]
		public string UserName { get; set; } = "";
		
		[BsonElement("roomId")]
		public string? RoomId { get; set; }
		
		[BsonElement("zone")]
		public string? Zone { get; set; }
		
		[BsonElement("region")]
		public string? Region { get; set; }
		
		[BsonElement("isPublic")]
		public bool? IsPublic { get; set; }
	}
}
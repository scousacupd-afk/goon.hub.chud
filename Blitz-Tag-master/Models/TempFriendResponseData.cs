namespace Blitz_Tag.Models
{
	public class TempFriendResponseData
	{
		public TempFriendPresenceResponseData Presence { get; set; } = new();

		public DateTime Created { get; set; }
	}
}
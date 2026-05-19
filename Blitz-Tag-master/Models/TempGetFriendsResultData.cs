namespace Blitz_Tag.Models
{
	public class TempGetFriendsResultData
	{
		public List<TempFriendResponseData> Friends { get; set; } = [];

		public PrivacyState MyPrivacyState { get; set; }
	}
}
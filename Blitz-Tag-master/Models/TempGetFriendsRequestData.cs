namespace Blitz_Tag.Models
{
	public class TempGetFriendsRequestData
	{
		public string PlayFabId { get; set; } = "";

		public string? MothershipId { get; set; }

		public string? MothershipToken { get; set; }

		public string PlayFabTicket { get; set; } = "";
	}
}
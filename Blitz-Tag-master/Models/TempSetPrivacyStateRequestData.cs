using System.Diagnostics.CodeAnalysis;

namespace Blitz_Tag.Models
{
	public class TempSetPrivacyStateRequestData
	{
		public string PlayFabId { get; set; } = "";

		public string PlayFabTicket { get; set; } = "";

		public string PrivacyState { get; set; } = "";
	}
}
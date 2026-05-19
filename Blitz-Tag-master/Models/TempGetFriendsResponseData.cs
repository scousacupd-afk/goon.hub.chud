using System.Diagnostics.CodeAnalysis;

namespace Blitz_Tag.Models
{
	public class TempGetFriendsResponseData
	{
		public TempGetFriendsResultData? Result { get; set; }

		public int StatusCode { get; set; }

		public string? Error { get; set; }
	}
}
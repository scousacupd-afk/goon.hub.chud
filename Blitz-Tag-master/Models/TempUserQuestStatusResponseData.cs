using System;
using Newtonsoft.Json;

namespace Blitz_Tag.Models
{
	public class TempUserQuestStatusResponseData
	{
		[JsonProperty(PropertyName = "dailyPoints")]
		public Dictionary<string, int> DailyPoints { get; set; } = [];
		
		[JsonProperty(PropertyName = "weeklyPoints")]
		public Dictionary<int, int> WeeklyPoints { get; set; } = [];
		
		[JsonProperty(PropertyName = "userPointsTotal")]
		public int UserPointsTotal { get; set; }
	}
}
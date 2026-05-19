namespace Blitz_Tag.Models
{
	[Flags]
	public enum AllowGrabbingFlags
	{
		None = 0,
		OutOfHands = 1,
		FromBags = 2,
		EntireBag = 4
	}
}
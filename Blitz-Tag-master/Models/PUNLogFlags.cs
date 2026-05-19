namespace Blitz_Tag.Models
{
	[Flags]
    public enum PUNLogFlags
	{
		SerializeView = 1,
		OwnershipTransfer = 2,
		OwnershipRequest = 4,
		OwnershipUpdate = 8,
		RPC = 16,
		Instantiate = 32,
		Destroy = 64,
		DestroyPlayer = 128
	}
}

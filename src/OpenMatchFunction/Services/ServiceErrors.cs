namespace OpenMatchFunction.Services;

public static class ServiceErrors
{
	public static readonly Google.Rpc.Status QueryError = new Google.Rpc.Status
	{
		Code	= (int)Code.FailedPrecondition,
		Message = "Failed Query Pools",
		Details = {}
	};
	
	public static readonly Google.Rpc.Status ProposalError = new Google.Rpc.Status
	{
		Code	= (int)Code.FailedPrecondition,
		Message = "Failed Match Proposals",
		Details = {}
	};
	
	public static readonly Google.Rpc.Status ValidationError = new Google.Rpc.Status
	{
		Code	= (int)Code.OutOfRange,
		Message = "Run request validation failed",
		Details = {}
	};
}
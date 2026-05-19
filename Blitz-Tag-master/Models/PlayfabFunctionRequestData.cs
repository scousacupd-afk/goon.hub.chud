namespace Blitz_Tag.Models
{
    public class PlayfabFunctionRequestData<T>
    {
        public EntityProfileBody CallerEntityProfile { get; set; } = new();

        public T? FunctionArgument { get; set; }
    }
    
    public class EntityProfileBody
    {
        public EntityLineage Lineage { get; set; } = new();
    }

    public class EntityLineage
    {
        public string MasterPlayerAccountId { get; set; } = "";
    }

    public class PlayfabFunctionRequestData : PlayfabFunctionRequestData<object>
    {
    }
}
